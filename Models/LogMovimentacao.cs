// Models/LogMovimentacao.cs
using System;
using System.ComponentModel.DataAnnotations;

public class LogMovimentacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MovimentacaoId { get; set; }

    [Required]
    public string Usuario { get; set; } // Quem realizou a movimentação

    public DateTime DataHora { get; set; } = DateTime.Now;

    [Required]
    public string Acao { get; set; } // Descrição da ação realizada, ex: "Compra realizada" ou "Reabastecimento"
}


