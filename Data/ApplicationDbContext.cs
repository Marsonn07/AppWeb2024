using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } 
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<LogMovimentacao> LogsMovimentacao { get; set; }
    public DbSet<CategoriaProduto> CategoriaProdutos { get; set; }
    public DbSet<Vendas> Vendas { get; set; }
}
