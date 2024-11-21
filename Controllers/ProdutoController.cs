using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ProdutoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public ProdutoController(ApplicationDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient(); // Cria uma instância de HttpClient para chamadas de API
    }

    // GET: Produto/Generate
    public async Task<IActionResult> Generate()
    {
        var produto = await ObterProdutoDeApiExternaAsync();
        if (produto != null)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            ViewData["Message"] = "Produto cadastrado com sucesso!";
        }
        else
        {
            ViewData["Message"] = "Falha ao obter produto da API.";
        }

        return View(produto);
    }

    private async Task<Produto> ObterProdutoDeApiExternaAsync()
    {
        // URL da API real para obter produtos
        string apiUrl = "https://fakestoreapi.com/products";
        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var produtosJson = JArray.Parse(content); // Usa Newtonsoft.Json para fazer parsing do JSON

                // Seleciona um produto aleatório da lista retornada pela API
                var random = new Random();
                var indexAleatorio = random.Next(produtosJson.Count);
                var produtoApi = produtosJson[indexAleatorio];

                // Cria um produto com os dados da API e valores aleatórios para os campos adicionais
                return new Produto
                {
                    Nome = produtoApi["title"]?.ToString() ?? "Produto Desconhecido",
                    Preco = Convert.ToDecimal(produtoApi["price"] ?? 0),
                    QuantidadeEmEstoque = random.Next(1, 100), // Gera uma quantidade aleatória
                    Descricao = produtoApi["description"]?.ToString() ?? $"Descrição gerada aleatoriamente {random.Next(1000)}"
                };
            }
        }
        catch (Exception ex)
        {
            // Trate erros conforme necessário
            Console.WriteLine($"Erro ao obter produto da API: {ex.Message}");
        }

        return null;
    }

    // ProdutoController.cs
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveGenerated([Bind("Nome,Preco,QuantidadeEmEstoque,Descricao")] Produto produto)
    {
        if (ModelState.IsValid)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Generate)); // Redireciona para a lista de produtos
        }
        return View("Generate", produto); // Retorna para a mesma View caso algo falhe
    }

}
