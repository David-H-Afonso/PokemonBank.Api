using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Domain.Entities;
using PokemonBank.Api.Infrastructure;

namespace PokemonBank.Api.Infrastructure.Services
{
    /// <summary>
    /// Service to monitor and automatically import Pokemon files from the Documents/Pokebank/storage directory
    /// </summary>
    public class FileWatcherService
    {
        private readonly AppDbContext _context;
        private readonly PkhexCoreParser _parser;
        private readonly FileStorageService _storage;
        private readonly string _watchPath;

        public FileWatcherService(AppDbContext context, PkhexCoreParser parser, FileStorageService storage)
        {
            _context = context;
            _parser = parser;
            _storage = storage;

            // Watch the Documents/Pokebank/storage directory
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _watchPath = Path.Combine(documentsPath, "Pokebank", "storage");

            // Ensure the directory exists
            if (!Directory.Exists(_watchPath))
            {
                Directory.CreateDirectory(_watchPath);
                Console.WriteLine($"Created watch directory: {_watchPath}");
            }
        }

        /// <summary>
        /// Scans the watch directory for new Pokemon files and imports them
        /// </summary>
        public async Task<ImportScanResult> ScanAndImportNewFilesAsync()
        {
            var result = new ImportScanResult();

            try
            {
                // Get all Pokemon files in the directory and subdirectories
                var pokemonFiles = Directory.GetFiles(_watchPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => IsPokemonFile(file))
                    .ToList();

                Console.WriteLine($"Found {pokemonFiles.Count} Pokemon files in watch directory");

                foreach (var filePath in pokemonFiles)
                {
                    try
                    {
                        await ProcessFileAsync(filePath, result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                        result.Errors.Add($"{Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning directory {_watchPath}: {ex.Message}");
                result.Errors.Add($"Directory scan error: {ex.Message}");
            }

            return result;
        }

        private async Task ProcessFileAsync(string filePath, ImportScanResult result)
        {
            var fileName = Path.GetFileName(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var sha256 = FileStorageService.ComputeSha256(fileBytes);

            // Check if file is already imported
            var existingFile = await _context.Files
                .FirstOrDefaultAsync(f => f.Sha256 == sha256);

            if (existingFile != null)
            {
                result.AlreadyImported.Add(fileName);
                return;
            }

            // Parse the Pokemon file
            var parseResult = await _parser.ParseAsync(fileBytes, fileName);
            if (parseResult == null)
            {
                result.Errors.Add($"{fileName}: Failed to parse Pokemon file");
                return;
            }

            // Save to database
            _context.Files.Add(parseResult.File);
            _context.Pokemon.Add(parseResult.Pokemon);

            if (parseResult.Stats != null)
                _context.Stats.Add(parseResult.Stats);

            if (parseResult.Moves.Any())
                _context.Moves.AddRange(parseResult.Moves);

            if (parseResult.RelearnMoves.Any())
                _context.RelearnMoves.AddRange(parseResult.RelearnMoves);

            await _context.SaveChangesAsync();

            result.NewlyImported.Add(fileName);
            Console.WriteLine($"Successfully imported: {fileName}");
        }

        private static bool IsPokemonFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pk1" or ".pk2" or ".pk3" or ".pk4" or ".pk5" or ".pk6" or ".pk7" or ".pk8" or ".pk9" => true,
                ".pb7" or ".pb8" => true, // Pokemon Box files
                ".ek1" or ".ek2" or ".ek3" or ".ek4" or ".ek5" or ".ek6" or ".ek7" or ".ek8" or ".ek9" => true, // Encrypted
                ".ekx" => true, // Encrypted batch
                _ => false
            };
        }
    }

    public class ImportScanResult
    {
        public List<string> NewlyImported { get; } = new();
        public List<string> AlreadyImported { get; } = new();
        public List<string> Errors { get; } = new();

        public int TotalProcessed => NewlyImported.Count + AlreadyImported.Count + Errors.Count;
    }
}
