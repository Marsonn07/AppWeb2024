// Models/Funcionario.cs
using System.ComponentModel.DataAnnotations;

public class Funcionario
{
    public int Id { get; set; } // Chave primária
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "O campo Endereço é obrigatório.")]
    public string? Endereco { get; set; }
    [Required(ErrorMessage = "O campo Celular é obrigatório.")]
    public string? Celular { get; set; }
}


