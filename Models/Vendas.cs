// Models/Vendas.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Vendas
{
    [Key]
    public int Id { get; set; }
    public int CategoriaProdutoId { get; set; } // Relacionamento com CategoriaProduto
    [ForeignKey("CategoriaProdutoId")]
    public CategoriaProduto CategoriaProduto { get; set; }
    [Required]
    public int QuantidadeVendida { get; set; }
    [Required]
    public decimal ValorTotal { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.Now;
}
