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
            })
            .WithName("GetFileById")
            .WithSummary("Download a stored file by its internal ID")
            .WithDescription("Returns the original uploaded file using the file ID. Useful for auditing or retrieving individual files.")
            .WithTags("Files")
            .Produces<byte[]>(200, "application/octet-stream")
            .Produces(404);

            app.MapGet("/export/{pokemonId:int}", async (int pokemonId, AppDbContext db) =>
            {
                var poke = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();
                var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == poke.FileId);
                if (file == null || file.RawBlob == null || file.RawBlob.Length == 0)
                    return Results.Problem("Backup not found in database.");

                // Validate PKM (optional)
                try
                {
                    var pk = PKHeX.Core.EntityFormat.GetFromBytes(file.RawBlob);
                    if (pk == null) return Results.Problem("The file is not a valid PKM.");
                }
                catch { return Results.Problem("The file is not a valid PKM."); }

                // Use the original file name for download
                var downloadName = file.FileName;
                return Results.File(file.RawBlob, "application/octet-stream", downloadName);
            })
            .WithName("ExportPokemonOriginal")
            .WithSummary("Download the original PKM file of a Pokémon")
            .WithDescription("Returns the original PKM file (.pk9, .pk8, etc.) stored in the database for the specified Pokémon. The file is identical to the one initially uploaded.")
            .WithTags("Files")
            .Produces<byte[]>(200, "application/octet-stream")
            .Produces(404)
            .Produces(500);

            app.MapGet("/export/database/{pokemonId:int}", async (int pokemonId, AppDbContext db, FileStorageService storage) =>
            {
                var poke = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();
                var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == poke.FileId);
                if (file == null) return Results.NotFound();
                var bytes = storage.Read(file.StoredPath);
                if (bytes == null || bytes.Length == 0) return Results.Problem("File not found on disk.");

                // Validate PKM (optional)
                try
                {
                    var pk = PKHeX.Core.EntityFormat.GetFromBytes(bytes);
                    if (pk == null) return Results.Problem("The file is not a valid PKM.");
                }
                catch { return Results.Problem("The file is not a valid PKM."); }

                // Use the original file name for download
                var downloadName = file.FileName;
                return Results.File(bytes, "application/octet-stream", downloadName);
            })
            .WithName("ExportPokemonFromDisk")
            .WithSummary("Download the PKM file from disk (audit)")
            .WithDescription("Returns the PKM file stored on disk for the specified Pokémon. Useful for comparing the database backup vs. the file on disk.")
            .WithTags("Files")
            .Produces<byte[]>(200, "application/octet-stream")
            .Produces(404)
            .Produces(500);
            return app;
        }
    }
}
