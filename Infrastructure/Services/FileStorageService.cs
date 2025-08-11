
using System.Security.Cryptography;

namespace PokemonBank.Api.Infrastructure.Services
{
    public class FileStorageService
    {
        public void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        private readonly string _basePath;
        public FileStorageService(string basePath) { _basePath = basePath; }

        public void EnsureVault() => Directory.CreateDirectory(_basePath);

        public static string ComputeSha256(byte[] bytes)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }


        public string Save(string sha256, string ext, byte[] bytes, string? pokemonName = null, DateTime? importDate = null)
        {
            // Normalizar extensión y hash
            ext = ext.TrimStart('.').ToLowerInvariant();
            var date = importDate ?? DateTime.UtcNow;
            var safeName = string.IsNullOrWhiteSpace(pokemonName) ? "pokemon" : SanitizeFileName(pokemonName);
            var shortHash = sha256.Length > 8 ? sha256[..8] : sha256;
            var sub = Path.Combine(_basePath, ext, date.Year.ToString());
            Directory.CreateDirectory(sub);
            var filePath = Path.Combine(sub, $"{safeName}_{shortHash}.{ext}");
            File.WriteAllBytes(filePath, bytes);
            return filePath;
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(" ", "_");
        }

        public byte[] Read(string path) => File.ReadAllBytes(path);
    }
}