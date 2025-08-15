using BeastVault.Api.Domain.Entities;

namespace BeastVault.Api.Infrastructure.Services
{
    /// <summary>
    /// Service to compare two Pokemon entities and identify differences
    /// </summary>
    public static class PokemonComparisonService
    {
        public class ComparisonResult
        {
            public List<string> Differences { get; set; } = new();
            public bool AreIdentical => !Differences.Any();
        }

        public static ComparisonResult Compare(PokemonEntity original, PokemonEntity received)
        {
            var result = new ComparisonResult();

            // Basic identity fields
            if (original.SpeciesId != received.SpeciesId)
                result.Differences.Add($"Species: {original.SpeciesId} → {received.SpeciesId}");

            if (original.Nickname != received.Nickname)
                result.Differences.Add($"Nickname: '{original.Nickname}' → '{received.Nickname}'");

            // Trainer information
            if (original.OtName != received.OtName)
                result.Differences.Add($"OT Name: '{original.OtName}' → '{received.OtName}'");

            if (original.Tid != received.Tid)
                result.Differences.Add($"TID: {original.Tid} → {received.Tid}");

            if (original.Sid != received.Sid)
                result.Differences.Add($"SID: {original.Sid} → {received.Sid}");

            if (original.OTGender != received.OTGender)
                result.Differences.Add($"OT Gender: {original.OTGender} → {received.OTGender}");

            if (original.OTLanguage != received.OTLanguage)
                result.Differences.Add($"OT Language: '{original.OTLanguage}' → '{received.OTLanguage}'");

            // Handler information (expected to change on trade)
            if (original.CurrentHandler != received.CurrentHandler)
                result.Differences.Add($"Current Handler: {original.CurrentHandler} → {received.CurrentHandler}");

            if (original.HandlingTrainerName != received.HandlingTrainerName)
                result.Differences.Add($"Handling Trainer Name: '{original.HandlingTrainerName}' → '{received.HandlingTrainerName}'");

            if (original.HandlingTrainerFriendship != received.HandlingTrainerFriendship)
                result.Differences.Add($"Handling Trainer Friendship: {original.HandlingTrainerFriendship} → {received.HandlingTrainerFriendship}");

            // Game origin (should NOT change)
            if (original.OriginGame != received.OriginGame)
                result.Differences.Add($"⚠️ Origin Game: {original.OriginGame} → {received.OriginGame} (UNEXPECTED!)");

            // Friendship/happiness
            if (original.CurrentFriendship != received.CurrentFriendship)
                result.Differences.Add($"Friendship: {original.CurrentFriendship} → {received.CurrentFriendship}");

            // Core identity that should NEVER change
            if (original.EncryptionConstant != received.EncryptionConstant)
                result.Differences.Add($"🚨 Encryption Constant: {original.EncryptionConstant} → {received.EncryptionConstant} (CRITICAL!)");

            if (original.PersonalityId != received.PersonalityId)
                result.Differences.Add($"🚨 PID: {original.PersonalityId} → {received.PersonalityId} (CRITICAL!)");

            // Other fields that might change
            if (original.Experience != received.Experience)
                result.Differences.Add($"Experience: {original.Experience} → {received.Experience}");

            if (original.Level != received.Level)
                result.Differences.Add($"Level: {original.Level} → {received.Level}");

            // Memory system (expected to change on trade)
            if (original.HandlingTrainerMemory != received.HandlingTrainerMemory)
                result.Differences.Add($"HT Memory: {original.HandlingTrainerMemory} → {received.HandlingTrainerMemory}");

            return result;
        }

        public static void LogComparison(PokemonEntity original, PokemonEntity received, string context = "")
        {
            var comparison = Compare(original, received);

            Console.WriteLine($"\n=== POKEMON COMPARISON {context} ===");
            Console.WriteLine($"Original: {PkHexStringService.GetSpeciesName(original.SpeciesId)} (ID: {original.Id})");
            Console.WriteLine($"Received: {PkHexStringService.GetSpeciesName(received.SpeciesId)} (ID: {received.Id})");

            if (comparison.AreIdentical)
            {
                Console.WriteLine("✅ Pokemon are identical!");
            }
            else
            {
                Console.WriteLine($"⚠️ Found {comparison.Differences.Count} differences:");
                foreach (var diff in comparison.Differences)
                {
                    Console.WriteLine($"  • {diff}");
                }
            }
            Console.WriteLine("================================\n");
        }
    }
}
