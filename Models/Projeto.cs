// Models/Projeto.cs
using System.ComponentModel.DataAnnotations;

public class Projeto
{
    [Key] // Define que esta propriedade é a chave primária
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
    [StringLength(200, ErrorMessage = "O campo Descrição não pode exceder 200 caracteres.")]
    public string Descricao { get; set; }
}


