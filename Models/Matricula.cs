// Models/Matricula.cs
using System;
using System.ComponentModel.DataAnnotations;

public class Matricula
{
    [Key] // Define que esta propriedade é a chave primária
    public int MatriculaId { get; set; }

    [Required(ErrorMessage = "O campo AlunoMatricula é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo AlunoMatricula deve ser maior que zero.")]
    public int AlunoMatricula { get; set; } // Referência sem FK direta

    [Required(ErrorMessage = "O campo ProjetoId é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo ProjetoId deve ser maior que zero.")]
    public int ProjetoId { get; set; } // Referência sem FK direta

    [Required(ErrorMessage = "O campo Data da Matrícula é obrigatório.")]
    [DataType(DataType.Date, ErrorMessage = "O campo Data da Matrícula deve ser uma data válida.")]
    public DateTime DataMatricula { get; set; }
}
