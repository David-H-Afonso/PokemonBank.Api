using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Infrastructure;
using PokemonBank.Api.Infrastructure.Services;

namespace PokemonBank.Api.Endpoints
{
    public static class FilesEndpoints
    {
        public static IEndpointRouteBuilder MapFilesEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/files/{id:int}", async (int id, AppDbContext db, FileStorageService storage) =>
            {
                var f = await db.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (f == null) return Results.NotFound();
                var bytes = storage.Read(f.StoredPath);
                var contentType = "application/octet-stream";
                var downloadName = f.FileName;
                return Results.File(bytes, contentType, downloadName);
            });
            // Exportar el archivo original (backup) desde la base de datos, extensión original
            app.MapGet("/export/{pokemonId:int}", async (int pokemonId, AppDbContext db) =>
            {
                var poke = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();
                var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == poke.FileId);
                if (file == null || file.RawBlob == null || file.RawBlob.Length == 0)
                    return Results.Problem("Backup no encontrado en base de datos.");

                // Validar PKM (opcional)
                try
                {
                    var pk = PKHeX.Core.EntityFormat.GetFromBytes(file.RawBlob);
                    if (pk == null) return Results.Problem("El archivo no es un PKM válido.");
                }
                catch { return Results.Problem("El archivo no es un PKM válido."); }

                var ext = string.IsNullOrWhiteSpace(file.Format) ? "pk9" : file.Format;
                var downloadName = $"pokemon_{pokemonId}.{ext}";
                return Results.File(file.RawBlob, "application/octet-stream", downloadName);
            });

            // Exportar el archivo desde disco (por si se requiere comparar o auditar)
            app.MapGet("/export/database/{pokemonId:int}", async (int pokemonId, AppDbContext db, FileStorageService storage) =>
            {
                var poke = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();
                var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == poke.FileId);
                if (file == null) return Results.NotFound();
                var bytes = storage.Read(file.StoredPath);
                if (bytes == null || bytes.Length == 0) return Results.Problem("Archivo no encontrado en disco.");

                // Validar PKM (opcional)
                try
                {
                    var pk = PKHeX.Core.EntityFormat.GetFromBytes(bytes);
                    if (pk == null) return Results.Problem("El archivo no es un PKM válido.");
                }
                catch { return Results.Problem("El archivo no es un PKM válido."); }

                var ext = string.IsNullOrWhiteSpace(file.Format) ? "pk9" : file.Format;
                var downloadName = $"pokemon_{pokemonId}.{ext}";
                return Results.File(bytes, "application/octet-stream", downloadName);
            });
            return app;
        }
    }
}
