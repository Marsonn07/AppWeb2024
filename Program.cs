using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppWeb2024.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do contexto de banco de dados com a string de conexão
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de controladores com as views
builder.Services.AddControllersWithViews();

// Adiciona suporte a autenticação de sessão se necessário
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Configuração do tempo de expiração
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuração do pipeline de requisições
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configuração de segurança para o ambiente de produção
}

app.UseHttpsRedirection(); // Redireciona para HTTPS
app.UseStaticFiles();      // Permite o uso de arquivos estáticos

app.UseRouting();

app.UseAuthentication();  // Necessário para autenticação de usuários
app.UseAuthorization();   // Necessário para autorização de usuários

app.UseSession(); // Habilita o uso de sessões

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
