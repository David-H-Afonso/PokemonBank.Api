using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Infrastructure;
using PokemonBank.Api.Endpoints;
using PokemonBank.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokemonBank.Api.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
    policy.WithOrigins(
        "http://localhost:3000",     // React default
        "http://localhost:5173",     // Vite default
        "http://localhost:8080",     // Vue CLI default
        "http://localhost:4200",     // Angular default
        "http://localhost:3001",     // Next.js alternate
        "http://localhost:5174",     // Vite alternate
        "https://localhost:3000",    // HTTPS versions
        "https://localhost:5173",
        "https://localhost:8080",
        "https://localhost:4200"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition", "Content-Length", "Content-Type");
    });
});

builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddPokemonBankServices(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Use CORS before other middleware
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();


app.MapHealthChecks();
app.MapImportEndpoints();
app.MapPokemonEndpoints();
app.MapFilesEndpoints();

// Asegurar que exista la carpeta de almacenamiento y la BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    var storage = scope.ServiceProvider.GetRequiredService<PokemonBank.Api.Infrastructure.Services.FileStorageService>();
    storage.EnsureVault();
}

await app.RunAsync();

namespace PokemonBank.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            var cs = config.GetConnectionString("Default")!;
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(cs);
            });
            return services;
        }

        public static IServiceCollection AddPokemonBankServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<FileStorageService>(sp =>
            {
                var basePath = config.GetSection("Vault").GetValue<string>("BasePath") ?? "Storage/Vault";
                return new FileStorageService(basePath);
            });
            services.AddScoped<PokemonBank.Api.Infrastructure.Services.PkhexCoreParser>();
            return services;
        }
    }
}
