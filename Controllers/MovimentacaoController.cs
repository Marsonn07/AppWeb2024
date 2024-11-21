using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class MovimentacaoController : Controller
{
    private readonly ApplicationDbContext _context;

    public MovimentacaoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Movimentacao/NovaMovimentacao
    public IActionResult NovaMovimentacao()
    {
        // Carrega a lista de produtos do banco de dados
        var produtos = _context.Produtos.ToList(); // Certifique-se de que _context é o seu ApplicationDbContext

        // Passa a lista para a View usando ViewData
        ViewData["Produtos"] = produtos;

        // Você também pode usar ViewBag se preferir: ViewBag.Produtos = produtos;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NovaMovimentacao(int produtoId, int quantidade, TipoMovimentacao tipo)
    {
        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto == null) return NotFound();

        // Atualiza a quantidade em estoque
        if (tipo == TipoMovimentacao.Saida && produto.QuantidadeEmEstoque < quantidade)
        {
            ModelState.AddModelError("", "Quantidade em estoque insuficiente.");
            return View();
        }

        // Ajusta a quantidade em estoque com base no tipo de movimentação
        produto.QuantidadeEmEstoque += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;

        // Cria a nova movimentação
        var movimentacao = new Movimentacao
        {
            ProdutoId = produtoId,
            Quantidade = quantidade,
            Tipo = tipo
        };

        // Adiciona e salva a movimentação para gerar o Id
        _context.Movimentacoes.Add(movimentacao);
        await _context.SaveChangesAsync(); // Salva para garantir que movimentacao.Id seja gerado

        // Captura o usuário autenticado ou define como "Desconhecido"
        var usuario = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Desconhecido";

        // Log de movimentação
        var log = new LogMovimentacao
        {
            MovimentacaoId = movimentacao.Id,
            Usuario = usuario,
            Acao = tipo == TipoMovimentacao.Entrada ? "Reabastecimento de Estoque" : "Venda de Produto",
            DataHora = DateTime.Now // Inclua o campo DataHora se necessário
        };

        // Adiciona e salva o log de movimentação
        _context.LogsMovimentacao.Add(log);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
