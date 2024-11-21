// Controllers/ProjetoController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

public class ProjetoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjetoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Método para verificar se o usuário está logado em cada requisição
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Verifica se há uma sessão de usuário ativa
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            // Redireciona para a página de login se o usuário não estiver logado
            context.Result = RedirectToAction("Login", "User");
        }
        base.OnActionExecuting(context);
    }

    // GET: Projeto
    public async Task<IActionResult> Index(int? id, string descricao)
    {
        var query = _context.Projetos.AsQueryable();

        // Filtra por Id se fornecido
        if (id.HasValue)
        {
            query = query.Where(p => p.Id == id.Value);
        }

        // Filtra por Descricao se fornecido
        if (!string.IsNullOrEmpty(descricao))
        {
            query = query.Where(p => p.Descricao.Contains(descricao));
        }

        return View(await query.ToListAsync());
    }

    // Método para mostrar o formulário de geração de dados aleatórios
    public IActionResult GenerateRandomData()
    {
        return View();
    }

    // POST: Geração de dados aleatórios
    [HttpPost]
    public async Task<IActionResult> GenerateRandomData(int quantidade)
    {
        if (quantidade < 1)
        {
            ModelState.AddModelError(string.Empty, "A quantidade deve ser maior que 0.");
            return View();
        }

        var random = new Random();
        for (int i = 0; i < quantidade; i++)
        {
            var projeto = new Projeto
            {
                Descricao = "Projeto " + random.Next(1, 1000)
            };
            _context.Projetos.Add(projeto);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Projeto/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var projeto = await _context.Projetos.FirstOrDefaultAsync(m => m.Id == id);
        if (projeto == null) return NotFound();

        return View(projeto);
    }

    // GET: Projeto/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Projeto/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Descricao")] Projeto projeto)
    {
        if (ModelState.IsValid)
        {
            _context.Add(projeto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(projeto);
    }

    // GET: Projeto/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var projeto = await _context.Projetos.FindAsync(id);
        if (projeto == null) return NotFound();

        return View(projeto);
    }

    // POST: Projeto/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao")] Projeto projeto)
    {
        if (id != projeto.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(projeto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjetoExists(projeto.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(projeto);
    }

    // GET: Projeto/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var projeto = await _context.Projetos.FirstOrDefaultAsync(m => m.Id == id);
        if (projeto == null) return NotFound();

        return View(projeto);
    }

    // POST: Projeto/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var projeto = await _context.Projetos.FindAsync(id);
        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProjetoExists(int id)
    {
        return _context.Projetos.Any(e => e.Id == id);
    }
}
