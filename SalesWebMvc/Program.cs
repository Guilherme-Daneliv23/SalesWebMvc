using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext com MySQL
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("SalesWebMvcContext") ?? throw new InvalidOperationException("Connection string 'SalesWebMvcContext' not found."),
        new MySqlServerVersion(new Version(8, 0, 21)) // Substitua pela versão do seu MySQL
    )
);

// Adiciona o SeedingService ao contêiner de injeção de dependência
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();

// Configura os serviços MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Verifica se está em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    // Inicia o SeedingService para popular o banco de dados
    using (var scope = app.Services.CreateScope())
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
        seedingService.Seed(); // Executa o método Seed
    }

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuração do pipeline de requisição HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();