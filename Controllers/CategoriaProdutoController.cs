using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class CategoriaProdutoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public CategoriaProdutoController(ApplicationDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateData()
    {
        string apiUrl = "https://fakestoreapi.com/products/categories";
        var response = await _httpClient.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var categoriasJson = JArray.Parse(content);

            foreach (var categoriaNome in categoriasJson)
            {
                // Verifica se a categoria já existe para evitar duplicação
                if (!_context.CategoriaProdutos.Any(c => c.Nome == categoriaNome.ToString()))
                {
                    var categoria = new CategoriaProduto
                    {
                        Nome = categoriaNome.ToString()
                    };

                    _context.CategoriaProdutos.Add(categoria);
                    await _context.SaveChangesAsync();

                    // Gera vendas aleatórias para cada categoria
                    for (int i = 0; i < 10; i++) // Cria 10 vendas aleatórias por categoria
                    {
                        var quantidade = new Random().Next(1, 100);
                        var valorTotal = new Random().Next(50, 500) * quantidade;

                        var venda = new Vendas
                        {
                            CategoriaProdutoId = categoria.Id,
                            QuantidadeVendida = quantidade,
                            ValorTotal = valorTotal
                        };

                        _context.Vendas.Add(venda);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Content("Dados gerados com sucesso!");
        }

        return Content("Falha ao obter categorias de produtos da API.");
    }


    // GET: CategoriaProduto/ResumoVendas
    public IActionResult ResumoVendas()
    {
        var resumo = _context.Vendas
            .Include(v => v.CategoriaProduto)
            .GroupBy(v => new { v.CategoriaProdutoId, v.CategoriaProduto.Nome })
            .Select(g => new
            {
                Categoria = g.Key.Nome,
                TotalQuantidade = g.Sum(v => v.QuantidadeVendida),
                TotalValor = g.Sum(v => v.ValorTotal)
            })
            .ToList();

        return View(resumo);
    }

    // GET: CategoriaProduto/PivotVendasPorAno
    public IActionResult PivotVendasPorAno()
    {
        var pivotData = _context.Vendas
            .Include(v => v.CategoriaProduto)
            .GroupBy(v => new { v.CategoriaProduto.Nome, Ano = v.DataVenda.Year })
            .Select(g => new
            {
                Categoria = g.Key.Nome,
                Ano = g.Key.Ano,
                TotalVendas = g.Sum(v => v.ValorTotal)
            })
            .ToList();

        // Transforma os dados em uma estrutura de pivotamento simples
        var pivotResult = pivotData
            .GroupBy(p => p.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                VendasPorAno = g.ToDictionary(x => x.Ano, x => x.TotalVendas)
            })
            .ToList();

        return View(pivotResult);
    }
}
