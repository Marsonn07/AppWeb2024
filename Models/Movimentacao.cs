// Models/Movimentacao.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Movimentacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProdutoId { get; set; }

    [ForeignKey("ProdutoId")]
    public Produto Produto { get; set; }

    [Required]
    public DateTime DataHora { get; set; } = DateTime.Now;

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public TipoMovimentacao Tipo { get; set; } // Entrada ou Saída

    public decimal ValorTotal => Quantidade * Produto.Preco;
}

public enum TipoMovimentacao
{
    Entrada,
    Saida
}


