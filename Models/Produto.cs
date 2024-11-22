// Models/Produto.cs
using System.ComponentModel.DataAnnotations;

public class Produto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; }

    [Required]
    public int QuantidadeEmEstoque { get; set; }

    [Required]
    public decimal Preco { get; set; }

    public string Descricao { get; set; }
}


