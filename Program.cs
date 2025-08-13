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
            // Use user's Documents folder for database as well
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var defaultDbPath = Path.Combine(documentsPath, "PokeBank", "pokemonbank.db");
            var configuredCs = config.GetConnectionString("Default");

            string connectionString;
            if (string.IsNullOrEmpty(configuredCs) || configuredCs.Contains("Storage/pokemonbank.db"))
            {
                // Ensure the directory exists
                var dbDirectory = Path.GetDirectoryName(defaultDbPath);
                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory!);
                }
                // Use default path in Documents if no custom connection string is configured
                connectionString = $"Data Source={defaultDbPath}";
            }
            else
            {
                connectionString = configuredCs;
            }

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(connectionString);
            });
            return services;
        }

        public static IServiceCollection AddPokemonBankServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<FileStorageService>(sp =>
            {
                // Use user's Documents folder by default for better security and user experience
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var defaultPath = Path.Combine(documentsPath, "PokeBank");
                var configuredPath = config.GetSection("Vault").GetValue<string>("BasePath");
                var basePath = string.IsNullOrWhiteSpace(configuredPath) ? defaultPath : configuredPath;

                // Ensure the base directory exists
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Console.WriteLine($"Created PokeBank directory: {basePath}");
                }

                return new FileStorageService(basePath);
            });
            services.AddScoped<PokemonBank.Api.Infrastructure.Services.PkhexCoreParser>();
            return services;
        }
    }
}
