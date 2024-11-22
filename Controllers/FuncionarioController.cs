// Controllers/FuncionarioController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

public class FuncionarioController : Controller
{
    private readonly ApplicationDbContext _context;

    public FuncionarioController(ApplicationDbContext context)
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

    // GET: Funcionario
    public async Task<IActionResult> Index(int? filterId, string? searchName)
    {
        // Verifica se um filtro de ID foi fornecido
        if (filterId.HasValue)
        {
            var filteredFuncionarios = await _context.Funcionarios
                .Where(f => f.Id == filterId.Value)
                .ToListAsync();

            return View(filteredFuncionarios);
        }

        // Filtra os funcionários pelo nome, se fornecido
        if (!string.IsNullOrEmpty(searchName))
        {
            var filteredFuncionarios = await _context.Funcionarios
                .Where(f => f.Nome.Contains(searchName))
                .ToListAsync();

            return View(filteredFuncionarios);
        }

        // Se nenhum filtro for fornecido, retorna todos os funcionários
        return View(await _context.Funcionarios.ToListAsync());
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
            var funcionario = new Funcionario
            {
                Nome = "Funcionário " + random.Next(1, 1000),
                Endereco = "Endereço " + random.Next(1, 1000),
                Celular = "(99) 99999-" + random.Next(1000, 9999)
            };
            _context.Funcionarios.Add(funcionario);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Funcionario/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(m => m.Id == id);
        if (funcionario == null) return NotFound();

        return View(funcionario);
    }

    // GET: Funcionario/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Funcionario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nome,Endereco,Celular")] Funcionario funcionario)
    {
        if (ModelState.IsValid)
        {
            _context.Add(funcionario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(funcionario);
    }

    // GET: Funcionario/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null) return NotFound();

        return View(funcionario);
    }

    // POST: Funcionario/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Endereco,Celular")] Funcionario funcionario)
    {
        if (id != funcionario.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(funcionario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionarioExists(funcionario.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(funcionario);
    }

    // GET: Funcionario/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(m => m.Id == id);
        if (funcionario == null) return NotFound();

        return View(funcionario);
    }

    // POST: Funcionario/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        _context.Funcionarios.Remove(funcionario);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FuncionarioExists(int id)
    {
        return _context.Funcionarios.Any(e => e.Id == id);
    }
}
