using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Infrastructure;
using PokemonBank.Api.Infrastructure.Services;

namespace PokemonBank.Api.Endpoints
{
    public static class ImportEndpoints
    {
        /// <summary>
        /// Importa uno o varios archivos .pk* de Pokémon. Usa multipart/form-data.
        /// </summary>
        public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/import",
                [Consumes("multipart/form-data")]
            async ([FromForm] IFormFileCollection files, AppDbContext db, FileStorageService storage, PkhexCoreParser parser) =>
            {
                if (files == null || files.Count == 0)
                    return Results.BadRequest("Sin archivos.");

                var imported = new List<object>();

                foreach (var f in files)
                {
                    if (f.Length == 0) continue;

                    await using var ms = new MemoryStream();
                    await f.CopyToAsync(ms);
                    var bytes = ms.ToArray();

                    // Log para verificar que guardamos los bytes originales
                    Console.WriteLine($"Archivo original: {f.FileName}, Tamaño: {bytes.Length} bytes, SHA256: {FileStorageService.ComputeSha256(bytes)}");

                    var parse = await parser.ParseAsync(bytes, f.FileName);
                    if (parse is null)
                    {
                        imported.Add(new { FileName = f.FileName, Status = "error", Message = "No se pudo parsear" });
                        continue;
                    }

                    // Dedupe por hash
                    var existingFile = await db.Files.FirstOrDefaultAsync(x => x.Sha256 == parse.File.Sha256);
                    if (existingFile != null)
                    {
                        imported.Add(new { FileName = f.FileName, Status = "duplicate", PokemonId = existingFile.Id });
                        continue;
                    }


                    // Determinar nombre para el archivo: nickname o especie
                    var pokeName = !string.IsNullOrWhiteSpace(parse.Pokemon.Nickname) ? parse.Pokemon.Nickname : $"{parse.Pokemon.SpeciesId}";
                    var ext = Path.GetExtension(f.FileName).TrimStart('.').ToLowerInvariant();
                    var storedPath = storage.Save(parse.File.Sha256, ext, bytes, pokeName);

                    // Verificar que se guardó correctamente
                    var savedBytes = storage.Read(storedPath);
                    var savedSha256 = FileStorageService.ComputeSha256(savedBytes);
                    Console.WriteLine($"Archivo guardado: {storedPath}, Tamaño: {savedBytes.Length} bytes, SHA256: {savedSha256}");
                    Console.WriteLine($"¿Son idénticos? {parse.File.Sha256 == savedSha256 && bytes.Length == savedBytes.Length}");

                    // Guardar backup en base de datos
                    parse.File.RawBlob = bytes;

                    // Persistencia
                    parse.File.StoredPath = storedPath;
                    db.Files.Add(parse.File);
                    await db.SaveChangesAsync();

                    parse.Pokemon.FileId = parse.File.Id;
                    db.Pokemon.Add(parse.Pokemon);
                    await db.SaveChangesAsync();

                    if (parse.Stats != null)
                    {
                        parse.Stats.PokemonId = parse.Pokemon.Id;
                        db.Stats.Add(parse.Stats);
                    }
                    if (parse.Moves.Any())
                    {
                        foreach (var m in parse.Moves)
                        {
                            m.PokemonId = parse.Pokemon.Id;
                            db.Moves.Add(m);
                        }
                    }
                    if (parse.RelearnMoves.Any())
                    {
                        foreach (var rm in parse.RelearnMoves)
                        {
                            rm.PokemonId = parse.Pokemon.Id;
                            db.RelearnMoves.Add(rm);
                        }
                    }
                    await db.SaveChangesAsync();

                    imported.Add(new
                    {
                        FileName = f.FileName,
                        Status = "imported",
                        PokemonId = parse.Pokemon.Id,
                        Pokemon = parse.Pokemon,
                        Stats = parse.Stats,
                        Moves = parse.Moves,
                        RelearnMoves = parse.RelearnMoves
                    });
                }

                return Results.Ok(imported);
            })
            .DisableAntiforgery();

            return app;
        }
    }
}
