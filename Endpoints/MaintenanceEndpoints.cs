using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Infrastructure;
using PokemonBank.Api.Infrastructure.Services;

namespace PokemonBank.Api.Endpoints
{
    public static class MaintenanceEndpoints
    {
        public static IEndpointRouteBuilder MapMaintenanceEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/maintenance/sync", async (AppDbContext db, FileStorageService storage) =>
            {
                var result = new SyncResult();

                try
                {
                    // Get all files in database
                    var allDbFiles = await db.Files.ToListAsync();
                    result.TotalFilesInDatabase = allDbFiles.Count;

                    foreach (var dbFile in allDbFiles)
                    {
                        // Check if the stored file still exists
                        var fileExists = !string.IsNullOrEmpty(dbFile.StoredPath) && File.Exists(dbFile.StoredPath);

                        if (!fileExists)
                        {
                            // File no longer exists, remove from database
                            var pokemon = await db.Pokemon.FirstOrDefaultAsync(p => p.FileId == dbFile.Id);
                            if (pokemon != null)
                            {
                                // Remove related data
                                var stats = await db.Stats.FirstOrDefaultAsync(s => s.PokemonId == pokemon.Id);
                                if (stats != null) db.Stats.Remove(stats);

                                var moves = await db.Moves.Where(m => m.PokemonId == pokemon.Id).ToListAsync();
                                db.Moves.RemoveRange(moves);

                                var relearnMoves = await db.RelearnMoves.Where(r => r.PokemonId == pokemon.Id).ToListAsync();
                                db.RelearnMoves.RemoveRange(relearnMoves);

                                db.Pokemon.Remove(pokemon);
                                result.RemovedPokemon.Add(pokemon.Nickname ?? $"Species #{pokemon.SpeciesId}");
                            }

                            db.Files.Remove(dbFile);
                            result.RemovedFiles.Add(dbFile.FileName);
                            Console.WriteLine($"Removed orphaned file from database: {dbFile.FileName} (path: {dbFile.StoredPath})");
                        }
                        else
                        {
                            result.ValidFiles.Add(dbFile.FileName);
                        }
                    }

                    if (result.RemovedFiles.Any() || result.RemovedPokemon.Any())
                    {
                        await db.SaveChangesAsync();
                        Console.WriteLine($"Database sync completed: Removed {result.RemovedFiles.Count} files and {result.RemovedPokemon.Count} Pokemon");
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during database sync: {ex.Message}");
                    return Results.Problem($"Sync failed: {ex.Message}");
                }
            })
            .WithName("SyncDatabase")
            .WithSummary("Synchronize database with file system")
            .WithDescription("Removes orphaned entries from database when files no longer exist on disk")
            .WithTags("Maintenance")
            .Produces<SyncResult>(200)
            .Produces(500);

            app.MapGet("/maintenance/status", async (AppDbContext db, FileStorageService storage) =>
            {
                var result = new StatusResult();

                try
                {
                    // Get database counts
                    result.TotalPokemonInDatabase = await db.Pokemon.CountAsync();
                    result.TotalFilesInDatabase = await db.Files.CountAsync();

                    // Check file system status
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var backupPath = Path.Combine(documentsPath, "Pokebank", "backup");

                    if (Directory.Exists(backupPath))
                    {
                        var pokemonFiles = Directory.GetFiles(backupPath, "*.*", SearchOption.AllDirectories)
                            .Where(file => IsPokemonFile(file))
                            .ToList();

                        result.TotalFilesInBackupDirectory = pokemonFiles.Count;
                        result.BackupDirectoryPath = backupPath;
                    }
                    else
                    {
                        result.TotalFilesInBackupDirectory = 0;
                        result.BackupDirectoryPath = $"{backupPath} (does not exist)";
                    }

                    // Check for orphaned entries
                    var allDbFiles = await db.Files.ToListAsync();
                    foreach (var dbFile in allDbFiles)
                    {
                        var fileExists = !string.IsNullOrEmpty(dbFile.StoredPath) && File.Exists(dbFile.StoredPath);
                        if (!fileExists)
                        {
                            result.OrphanedFiles.Add(new OrphanedFileInfo
                            {
                                FileName = dbFile.FileName,
                                StoredPath = dbFile.StoredPath ?? "null",
                                Id = dbFile.Id
                            });
                        }
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting maintenance status: {ex.Message}");
                    return Results.Problem($"Status check failed: {ex.Message}");
                }
            })
            .WithName("GetMaintenanceStatus")
            .WithSummary("Get database and file system status")
            .WithDescription("Shows counts and identifies orphaned entries")
            .WithTags("Maintenance")
            .Produces<StatusResult>(200)
            .Produces(500);

            return app;
        }

        private static bool IsPokemonFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pk1" or ".pk2" or ".pk3" or ".pk4" or ".pk5" or ".pk6" or ".pk7" or ".pk8" or ".pk9" => true,
                ".pgt" or ".pgf" or ".wc3" or ".wc4" or ".wc5" or ".wc6" or ".wc7" or ".wc8" or ".wc9" => true,
                _ => false
            };
        }
    }

    public class SyncResult
    {
        public int TotalFilesInDatabase { get; set; }
        public List<string> RemovedFiles { get; set; } = new();
        public List<string> RemovedPokemon { get; set; } = new();
        public List<string> ValidFiles { get; set; } = new();
        public string Summary => $"Removed {RemovedFiles.Count} orphaned files and {RemovedPokemon.Count} Pokemon. {ValidFiles.Count} files remain valid.";
    }

    public class StatusResult
    {
        public int TotalPokemonInDatabase { get; set; }
        public int TotalFilesInDatabase { get; set; }
        public int TotalFilesInBackupDirectory { get; set; }
        public string BackupDirectoryPath { get; set; } = string.Empty;
        public List<OrphanedFileInfo> OrphanedFiles { get; set; } = new();
        public bool IsInSync => OrphanedFiles.Count == 0;
        public string Summary => $"Database: {TotalPokemonInDatabase} Pokemon, {TotalFilesInDatabase} files. Directory: {TotalFilesInBackupDirectory} files. Orphaned: {OrphanedFiles.Count}";
    }

    public class OrphanedFileInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StoredPath { get; set; } = string.Empty;
    }
}
