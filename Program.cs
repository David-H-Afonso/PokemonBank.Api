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
        // Para Electron y desarrollo local, permitir cualquier localhost
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin)) return false;
            var uri = new Uri(origin);
            return uri.Host == "localhost" || uri.Host == "127.0.0.1";
        })
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
app.MapScanEndpoints();
app.MapMaintenanceEndpoints();

// Asegurar que exista la carpeta de almacenamiento y la BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    var storage = scope.ServiceProvider.GetRequiredService<PokemonBank.Api.Infrastructure.Services.FileStorageService>();
    storage.EnsureVault();

    // Automatically scan for new files on startup
    var fileWatcher = scope.ServiceProvider.GetRequiredService<PokemonBank.Api.Infrastructure.Services.FileWatcherService>();
    var scanResult = await fileWatcher.ScanAndImportNewFilesAsync();
    if (scanResult.NewlyImported.Any())
    {
        Console.WriteLine($"Startup scan: Imported {scanResult.NewlyImported.Count} new Pokemon files");
    }
}

await app.RunAsync();

namespace PokemonBank.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            // Priorizar variable de entorno DB_PATH para Electron
            var envDbPath = Environment.GetEnvironmentVariable("DB_PATH");
            string dbPath;

            if (!string.IsNullOrEmpty(envDbPath))
            {
                dbPath = envDbPath;
            }
            else
            {
                // Use LocalAppData for database (private data) como fallback
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                dbPath = Path.Combine(localAppData, "Pokebank", "storage", "pokemonbank.db");
            }

            var configuredCs = config.GetConnectionString("Default");

            string connectionString;
            if (string.IsNullOrEmpty(configuredCs))
            {
                // Ensure the directory exists
                var dbDirectory = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory!);
                    Console.WriteLine($"Created database directory: {dbDirectory}");
                }
                connectionString = $"Data Source={dbPath}";
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
                // Priorizar variable de entorno STORAGE_PATH para Electron
                var envStoragePath = Environment.GetEnvironmentVariable("STORAGE_PATH");
                string basePath;

                if (!string.IsNullOrEmpty(envStoragePath))
                {
                    basePath = envStoragePath;
                }
                else
                {
                    // Use user's Documents folder for Pokemon file storage (public data) como fallback
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var defaultPath = Path.Combine(documentsPath, "Pokebank", "backup");
                    var configuredPath = config.GetSection("Vault").GetValue<string>("BasePath");
                    basePath = string.IsNullOrWhiteSpace(configuredPath) ? defaultPath : configuredPath;
                }

                // Ensure the base directory exists
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Console.WriteLine($"Created PokeBank backup directory: {basePath}");
                }

                return new FileStorageService(basePath);
            });
            services.AddScoped<PokemonBank.Api.Infrastructure.Services.PkhexCoreParser>();
            services.AddScoped<PokemonBank.Api.Infrastructure.Services.FileWatcherService>();
            return services;
        }
    }
}
