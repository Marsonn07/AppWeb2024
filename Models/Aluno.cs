// Models/Aluno.cs
using System;
using System.ComponentModel.DataAnnotations;

public class Aluno
{
    [Key] // Define que esta propriedade é a chave primária
    public int Matricula { get; set; }

    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }

    [StringLength(100, ErrorMessage = "O Nome do Responsável não pode exceder 100 caracteres.")]
    public string NomeResponsavel { get; set; }
}
