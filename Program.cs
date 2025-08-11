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

builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddPokemonBankServices(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
