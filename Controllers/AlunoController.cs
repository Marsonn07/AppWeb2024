// Controllers/AlunoController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

public class AlunoController : Controller
{
    private readonly ApplicationDbContext _context;

    public AlunoController(ApplicationDbContext context)
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


    // GET: Aluno
    public async Task<IActionResult> Index(int? filterId)
    {
        // Verifica se há um filtro fornecido
        if (filterId.HasValue)
        {
            // Filtra a lista de alunos pela matricula fornecida
            var filteredAlunos = await _context.Alunos
                .Where(a => a.Matricula == filterId.Value)
                .ToListAsync();

            return View(filteredAlunos);
        }

        // Se nenhum filtro for fornecido, retorna todos os alunos
        return View(await _context.Alunos.ToListAsync());
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
            var aluno = new Aluno
            {
                Nome = "Aluno " + random.Next(1, 1000),
                DataNascimento = DateTime.Today.AddYears(-random.Next(18, 30)).AddDays(random.Next(-365, 365)),
                NomeResponsavel = "Responsável " + random.Next(1, 1000)
            };
            _context.Alunos.Add(aluno);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Aluno/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var aluno = await _context.Alunos.FirstOrDefaultAsync(m => m.Matricula == id);
        if (aluno == null) return NotFound();

        return View(aluno);
    }

    // GET: Aluno/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Aluno/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,DataNascimento,NomeResponsavel")] Aluno aluno)
    {
        if (ModelState.IsValid)
        {
            _context.Add(aluno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aluno);
    }

    // GET: Aluno/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null) return NotFound();

        return View(aluno);
    }

    // POST: Aluno/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Matricula,Nome,DataNascimento,NomeResponsavel")] Aluno aluno)
    {
        if (id != aluno.Matricula) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aluno);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlunoExists(aluno.Matricula)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(aluno);
    }

    // GET: Aluno/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var aluno = await _context.Alunos.FirstOrDefaultAsync(m => m.Matricula == id);
        if (aluno == null) return NotFound();

        return View(aluno);
    }

    // POST: Aluno/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AlunoExists(int id)
    {
        return _context.Alunos.Any(e => e.Matricula == id);
    }
}
