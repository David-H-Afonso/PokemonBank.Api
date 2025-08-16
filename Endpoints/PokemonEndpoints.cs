
using Microsoft.EntityFrameworkCore;
using BeastVault.Api.Contracts;
using BeastVault.Api.Infrastructure;
using BeastVault.Api.Infrastructure.Services;
using BeastVault.Api.Domain.Services;
using BeastVault.Api.Domain.ValueObjects;


namespace BeastVault.Api.Endpoints
{
    public static class PokemonEndpoints
    {
        public static IEndpointRouteBuilder MapPokemonEndpoints(this IEndpointRouteBuilder app)
        {
            // Admin endpoint to wipe the entire database (dangerous!)
            app.MapPost("/admin/wipe-database", async (AppDbContext db, FileStorageService storage) =>
            {
                // Get all files to delete their backups
                var allFiles = await db.Files.ToListAsync();

                // Remove all data from database
                db.Pokemon.RemoveRange(db.Pokemon);
                db.PokemonTags.RemoveRange(db.PokemonTags);
                db.Stats.RemoveRange(db.Stats);
                db.Moves.RemoveRange(db.Moves);
                db.RelearnMoves.RemoveRange(db.RelearnMoves);
                db.Files.RemoveRange(db.Files);
                db.Tags.RemoveRange(db.Tags);
                await db.SaveChangesAsync();

                // Delete all backup files
                int deletedBackups = 0;
                foreach (var file in allFiles)
                {
                    if (!string.IsNullOrEmpty(file.OriginalFileName))
                    {
                        try
                        {
                            var ext = Path.GetExtension(file.OriginalFileName);
                            storage.DeleteBackup(file.OriginalFileName, ext);
                            deletedBackups++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not delete backup for {file.OriginalFileName}: {ex.Message}");
                        }
                    }
                }

                return Results.Ok(new { Message = "Database wiped.", DeletedBackups = deletedBackups });
            })
            .WithName("WipeDatabase")
            .WithSummary("⚠️ ADMIN: Delete entire database")
            .WithDescription("DANGEROUS: Removes all Pokémon, files and data from the database. For development/testing only.")
            .WithTags("Admin")
            .Produces<string>(200);
            // Eliminar de la base de datos y archivo principal (conserva backup)
            app.MapDelete("/pokemon/{pokemonId:int}/database", async (int pokemonId, AppDbContext db, FileStorageService storage) =>
            {
                var poke = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();

                // Obtener el archivo asociado antes de eliminar
                var file = await db.Files.FirstOrDefaultAsync(f => f.Id == poke.FileId);

                // Eliminar datos relacionados
                var stats = await db.Stats.Where(s => s.PokemonId == pokemonId).ToListAsync();
                db.Stats.RemoveRange(stats);
                var moves = await db.Moves.Where(m => m.PokemonId == pokemonId).ToListAsync();
                db.Moves.RemoveRange(moves);
                var relearnMoves = await db.RelearnMoves.Where(rm => rm.PokemonId == pokemonId).ToListAsync();
                db.RelearnMoves.RemoveRange(relearnMoves);

                // Eliminar Pokemon
                db.Pokemon.Remove(poke);

                // Eliminar archivo físico principal si existe (preserva backup)
                bool fileDeleted = false;
                if (file != null)
                {
                    Console.WriteLine($"Attempting to delete main file: {file.FileName}");
                    Console.WriteLine($"Stored path: {file.StoredPath}");
                    Console.WriteLine($"File exists: {File.Exists(file.StoredPath)}");

                    try
                    {
                        storage.Delete(file.StoredPath);
                        fileDeleted = true;
                        Console.WriteLine($"Main file deleted successfully: {file.StoredPath} (backup preserved)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not delete main file {file.StoredPath}: {ex.Message}");
                        Console.WriteLine($"Exception details: {ex}");
                    }

                    db.Files.Remove(file);
                }
                else
                {
                    Console.WriteLine("No file record found for this Pokemon");
                }

                await db.SaveChangesAsync();
                return Results.Ok(new { Deleted = true, FileDeleted = fileDeleted, BackupPreserved = true });
            })
            .WithName("DeletePokemonFromDatabase")
            .WithSummary("Delete a Pokémon and its main file (preserves backup)")
            .WithDescription("Removes the Pokémon, all its related data from the database, and deletes the main file on disk. Backup file is preserved.")
            .WithTags("Pokemon", "Admin")
            .Produces<object>(200)
            .Produces(404);

            // Eliminar de base de datos y backup/disco
            app.MapDelete("/pokemon/{pokemonId:int}/backup", async (int pokemonId, AppDbContext db, FileStorageService storage) =>
            {
                var poke = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();

                // Obtener el archivo asociado antes de eliminar
                var file = await db.Files.FirstOrDefaultAsync(f => f.Id == poke.FileId);

                // Eliminar datos relacionados
                var stats = await db.Stats.Where(s => s.PokemonId == pokemonId).ToListAsync();
                db.Stats.RemoveRange(stats);
                var moves = await db.Moves.Where(m => m.PokemonId == pokemonId).ToListAsync();
                db.Moves.RemoveRange(moves);
                var relearnMoves = await db.RelearnMoves.Where(rm => rm.PokemonId == pokemonId).ToListAsync();
                db.RelearnMoves.RemoveRange(relearnMoves);

                // Eliminar Pokemon
                db.Pokemon.Remove(poke);

                // Eliminar archivo físico principal si existe
                bool fileDeleted = false;
                bool backupDeleted = false;
                if (file != null)
                {
                    Console.WriteLine($"Attempting to delete file: {file.FileName}");
                    Console.WriteLine($"Stored path: {file.StoredPath}");
                    Console.WriteLine($"File exists: {File.Exists(file.StoredPath)}");

                    try
                    {
                        storage.Delete(file.StoredPath);
                        fileDeleted = true;
                        Console.WriteLine($"Physical file deleted successfully: {file.StoredPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not delete physical file {file.StoredPath}: {ex.Message}");
                        Console.WriteLine($"Exception details: {ex}");
                    }

                    // Eliminar backup si existe
                    if (!string.IsNullOrEmpty(file.OriginalFileName))
                    {
                        try
                        {
                            var ext = Path.GetExtension(file.OriginalFileName);
                            storage.DeleteBackup(file.OriginalFileName, ext);
                            backupDeleted = true;
                            Console.WriteLine($"Backup file deleted successfully: {file.OriginalFileName}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not delete backup file {file.OriginalFileName}: {ex.Message}");
                        }
                    }

                    db.Files.Remove(file);
                }
                else
                {
                    Console.WriteLine("No file record found for this Pokemon");
                }

                await db.SaveChangesAsync();
                return Results.Ok(new { Deleted = true, FileDeleted = fileDeleted, BackupDeleted = backupDeleted, FileName = file?.FileName });
            })
            .WithName("DeletePokemonAndBackup")
            .WithSummary("Delete a Pokémon completely (database + file)")
            .WithDescription("Removes the Pokémon, all its related data and the original file from disk. Irreversible operation.")
            .WithTags("Pokemon", "Admin")
            .Produces<object>(200)
            .Produces(404);

            // ...resto de endpoints (get, patch, etc.)...

            // Modern advanced Pokemon query endpoint
            app.MapGet("/pokemon/advanced", async (AppDbContext db, [AsParameters] AdvancedPokemonQuery q) =>
            {
                var baseQuery = db.Pokemon.AsNoTracking().AsQueryable();
                
                // Apply advanced filtering and sorting
                var query = PokemonQueryService.BuildQuery(baseQuery, q);
                
                var total = await query.CountAsync();
                var items = await query
                    .Skip(q.Skip)
                    .Take(q.Take)
                    .Select(p => new PokemonListItemDto
                    {
                        Id = p.Id,
                        SpeciesId = p.SpeciesId,
                        Form = p.Form,
                        Nickname = p.Nickname,
                        Level = p.Level,
                        IsShiny = p.IsShiny,
                        BallId = p.BallId,
                        TeraType = p.TeraType,
                        SpriteKey = p.SpriteKey
                    })
                    .ToListAsync();

                // Get query statistics for monitoring
                var stats = PokemonQueryService.GetQueryStats(q);

                return Results.Ok(new 
                { 
                    Items = items, 
                    Total = total,
                    Stats = stats
                });
            })
            .WithName("GetAdvancedPokemonList")
            .WithSummary("Get Pokemon with advanced filtering, sorting and pagination")
            .WithDescription("Advanced endpoint with comprehensive filtering by types, generations, stats, and flexible sorting options.")
            .WithTags("Pokemon")
            .Produces<object>(200);

            // Legacy Pokemon endpoint (for backward compatibility)
            app.MapGet("/pokemon", async (AppDbContext db, [AsParameters] PokemonQuery q) =>
            {
                var query = db.Pokemon.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(q.Search))
                    query = query.Where(p => (p.Nickname ?? "").Contains(q.Search) || p.OtName.Contains(q.Search));
                if (q.SpeciesId.HasValue)
                    query = query.Where(p => p.SpeciesId == q.SpeciesId);
                if (q.Form.HasValue)
                    query = query.Where(p => p.Form == q.Form);
                if (q.IsShiny.HasValue)
                    query = query.Where(p => p.IsShiny == q.IsShiny);
                if (q.BallId.HasValue)
                    query = query.Where(p => p.BallId == q.BallId);
                if (q.OriginGame.HasValue)
                    query = query.Where(p => p.OriginGame == q.OriginGame);
                if (q.TeraType.HasValue)
                    query = query.Where(p => p.TeraType == q.TeraType);

                var total = await query.CountAsync();
                var items = await query
                    .OrderByDescending(p => p.Id)
                    .Skip(q.Skip)
                    .Take(q.Take)
                    .Select(p => new PokemonListItemDto
                    {
                        Id = p.Id,
                        SpeciesId = p.SpeciesId,
                        Form = p.Form,
                        Nickname = p.Nickname,
                        Level = p.Level,
                        IsShiny = p.IsShiny,
                        BallId = p.BallId,
                        TeraType = p.TeraType,
                        SpriteKey = p.SpriteKey
                    })
                    .ToListAsync();

                return Results.Ok(new PagedResult<PokemonListItemDto>(items, total));
            })
            .WithName("GetPokemonList")
            .WithSummary("Get a paginated list of Pokémon (Legacy)")
            .WithDescription("Legacy endpoint for backward compatibility. Use /pokemon/advanced for more features.")
            .WithTags("Pokemon", "Legacy")
            .Produces<PagedResult<PokemonListItemDto>>(200);

            // Metadata endpoint for frontend helpers
            app.MapGet("/pokemon/metadata", () =>
            {
                var types = PokemonGameInfoService.GetAllTypes().ToList();
                var generations = Enumerable.Range(1, 9).ToList();
                var genders = new[]
                {
                    new { Id = 0, Name = "Unknown" },
                    new { Id = 1, Name = "Male" },
                    new { Id = 2, Name = "Female" }
                };
                var sortFields = Enum.GetValues<PokemonSortField>()
                    .Select(f => new { Name = f.ToString(), Value = (int)f })
                    .ToList();
                var typeFilterModes = Enum.GetValues<TypeFilterMode>()
                    .Select(m => new { Name = m.ToString(), Value = (int)m })
                    .ToList();

                return Results.Ok(new
                {
                    Types = types,
                    Generations = generations,
                    Genders = genders,
                    SortFields = sortFields,
                    TypeFilterModes = typeFilterModes,
                    DefaultPageSize = 50,
                    MaxPageSize = 500
                });
            })
            .WithName("GetPokemonMetadata")
            .WithSummary("Get metadata for Pokemon filtering and sorting")
            .WithDescription("Returns available options for types, generations, sort fields, and other filter metadata.")
            .WithTags("Pokemon", "Metadata")
            .Produces<object>(200);

            app.MapGet("/pokemon/{id:int}", async (int id, AppDbContext db) =>
            {
                var p = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                var stats = await db.Stats.AsNoTracking().FirstOrDefaultAsync(x => x.PokemonId == p.Id);
                var moves = await db.Moves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                var relearnMoves = await db.RelearnMoves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                return Results.Ok(new PokemonDetailDto(p, stats, moves, relearnMoves));
            })
            .WithName("GetPokemonById")
            .WithSummary("Get complete details of a Pokémon")
            .WithDescription("Returns all data of a specific Pokémon including stats, moves and relearn moves.")
            .WithTags("Pokemon")
            .Produces<PokemonDetailDto>(200)
            .Produces(404);

            app.MapGet("/pokemon/{id:int}/showdown", async (int id, AppDbContext db) =>
            {
                var p = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                var stats = await db.Stats.AsNoTracking().FirstOrDefaultAsync(x => x.PokemonId == p.Id);
                var moves = await db.Moves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                var text = BeastVault.Api.Domain.ValueObjects.ShowdownExport.From(p, stats, moves);
                return Results.Text(text);
            })
            .WithName("ExportPokemonShowdown")
            .WithSummary("Export a Pokémon in Pokémon Showdown format")
            .WithDescription("Generates a Pokémon Showdown set with all the Pokémon data (moves, stats, item, etc.).")
            .WithTags("Pokemon")
            .Produces<string>(200, "text/plain")
            .Produces(404);

            app.MapPatch("/pokemon/{id:int}", async (int id, UpdatePokemonDto dto, AppDbContext db) =>
            {
                var p = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                if (dto.Favorite.HasValue) p.Favorite = dto.Favorite.Value;
                if (dto.Notes is not null) p.Notes = dto.Notes;
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("UpdatePokemon")
            .WithSummary("Update Pokémon properties")
            .WithDescription("Allows updating editable fields like favorite and notes. Only provided fields in the DTO are updated.")
            .WithTags("Pokemon")
            .Accepts<UpdatePokemonDto>("application/json")
            .Produces(204)
            .Produces(404);

            // Compare two Pokemon to see differences (useful for debugging trades)
            app.MapGet("/pokemon/compare/{id1:int}/{id2:int}", async (int id1, int id2, AppDbContext db) =>
            {
                var p1 = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id1);
                var p2 = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id2);

                if (p1 == null || p2 == null)
                    return Results.NotFound("One or both Pokemon not found");

                var comparison = PokemonComparisonService.Compare(p1, p2);

                return Results.Ok(new
                {
                    Pokemon1 = new { Id = p1.Id, Species = PkHexStringService.GetSpeciesName(p1.SpeciesId), Nickname = p1.Nickname },
                    Pokemon2 = new { Id = p2.Id, Species = PkHexStringService.GetSpeciesName(p2.SpeciesId), Nickname = p2.Nickname },
                    AreIdentical = comparison.AreIdentical,
                    Differences = comparison.Differences,
                    Summary = comparison.AreIdentical ? "Pokemon are identical" : $"Found {comparison.Differences.Count} differences"
                });
            })
            .WithName("ComparePokemon")
            .WithSummary("Compare two Pokémon and show differences")
            .WithDescription("Analyzes and compares all fields of two different Pokémon. Useful for detecting changes after trades or edits.")
            .WithTags("Pokemon", "Comparison")
            .Produces<object>(200)
            .Produces(404);

            return app;
        }
    }
}
