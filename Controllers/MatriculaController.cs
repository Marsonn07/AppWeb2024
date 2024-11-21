// Controllers/MatriculaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

public class MatriculaController : Controller
{
    private readonly ApplicationDbContext _context;

    public MatriculaController(ApplicationDbContext context)
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

    // GET: Matricula
    public async Task<IActionResult> Index(int? alunoMatricula, int? projetoId)
    {
        var query = _context.Matriculas.AsQueryable();

        // Filtra por AlunoMatricula se fornecido
        if (alunoMatricula.HasValue)
        {
            query = query.Where(m => m.AlunoMatricula == alunoMatricula.Value);
        }

        // Filtra por ProjetoId se fornecido
        if (projetoId.HasValue)
        {
            query = query.Where(m => m.ProjetoId == projetoId.Value);
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
        var alunos = await _context.Alunos.ToListAsync(); // Lista de alunos
        var projetos = await _context.Projetos.ToListAsync(); // Lista de projetos

        // Verifica se existem alunos e projetos
        if (!alunos.Any() || !projetos.Any())
        {
            ModelState.AddModelError(string.Empty, "Não há alunos ou projetos para associar.");
            return View();
        }

        for (int i = 0; i < quantidade; i++)
        {
            var matricula = new Matricula
            {
                AlunoMatricula = alunos[random.Next(alunos.Count)].Matricula, // Seleciona um aluno aleatório
                ProjetoId = projetos[random.Next(projetos.Count)].Id, // Seleciona um projeto aleatório
                DataMatricula = DateTime.Today.AddDays(-random.Next(1, 365)) // Data de matrícula aleatória
            };
            _context.Matriculas.Add(matricula);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // GET: Matricula/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.MatriculaId == id);
        if (matricula == null) return NotFound();

        return View(matricula);
    }

    // GET: Matricula/Create
    public async Task<IActionResult> Create()
    {
        // Carrega listas de alunos e projetos usando ViewBag
        ViewBag.Alunos = await _context.Alunos.Select(a => new { a.Matricula, a.Nome }).ToListAsync();
        ViewBag.Projetos = await _context.Projetos.Select(p => new { p.Id, p.Descricao }).ToListAsync();
        return View();
    }

    // POST: Matricula/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MatriculaId,AlunoMatricula,ProjetoId,DataMatricula")] Matricula matricula)
    {
        if (ModelState.IsValid)
        {
            _context.Add(matricula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(matricula);
    }

    // GET: Matricula/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula == null) return NotFound();

        return View(matricula);
    }

    // POST: Matricula/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MatriculaId,AlunoMatricula,ProjetoId,DataMatricula")] Matricula matricula)
    {
        if (id != matricula.MatriculaId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(matricula);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatriculaExists(matricula.MatriculaId)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(matricula);
    }

    // GET: Matricula/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.MatriculaId == id);
        if (matricula == null) return NotFound();

        return View(matricula);
    }

    // POST: Matricula/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        _context.Matriculas.Remove(matricula);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MatriculaExists(int id)
    {
        return _context.Matriculas.Any(e => e.MatriculaId == id);
    }

    // GET: Matricula/TotalizarDados
    public async Task<IActionResult> TotalizarDados(int? projetoId)
    {
        var query = _context.Matriculas
            .Join(_context.Alunos, m => m.AlunoMatricula, a => a.Matricula, (m, a) => new { m, a })
            .Join(_context.Projetos, ma => ma.m.ProjetoId, p => p.Id, (ma, p) => new { ma.m, ma.a, p })
            .GroupBy(ma => new { ma.a.Matricula, ma.a.Nome, ma.p.Descricao })
            .Select(g => new
            {
                AlunoMatricula = g.Key.Matricula,
                AlunoNome = g.Key.Nome,
                ProjetoDescricao = g.Key.Descricao,
                TotalMatriculas = g.Count()
            });

        // Filtra pelo projetoId, se fornecido
        if (projetoId.HasValue)
        {
            query = query.Where(q => q.ProjetoDescricao.Contains(projetoId.Value.ToString()));
        }

        // Execute a consulta e retorna para a View
        var resultado = await query.ToListAsync();

        return View(resultado);
    }
}
