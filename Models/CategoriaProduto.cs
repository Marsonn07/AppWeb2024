// Models/CategoriaProduto.cs
using System.ComponentModel.DataAnnotations;

public class CategoriaProduto
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nome { get; set; }
}


