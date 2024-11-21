using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppWeb2024.Models;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do contexto de banco de dados com a string de conex�o
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura��o de controladores com as views
builder.Services.AddControllersWithViews();

// Adiciona suporte a autentica��o de sess�o se necess�rio
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Configura��o do tempo de expira��o
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configura��o do pipeline de requisi��es
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configura��o de seguran�a para o ambiente de produ��o
}

app.UseHttpsRedirection(); // Redireciona para HTTPS
app.UseStaticFiles();      // Permite o uso de arquivos est�ticos

app.UseRouting();

app.UseAuthentication();  // Necess�rio para autentica��o de usu�rios
app.UseAuthorization();   // Necess�rio para autoriza��o de usu�rios

app.UseSession(); // Habilita o uso de sess�es

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
