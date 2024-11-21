// Models/User.cs
using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } // Armazenar a senha criptografada

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
