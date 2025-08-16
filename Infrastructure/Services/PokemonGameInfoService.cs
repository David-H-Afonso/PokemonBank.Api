using BeastVault.Api.Domain.ValueObjects;
using BeastVault.Api.Infrastructure.Services;
using PKHeX.Core;
using GameInfoDomain = BeastVault.Api.Domain.ValueObjects.GameInfo;

namespace BeastVault.Api.Infrastructure.Services;

/// <summary>
/// Service for providing Pokemon game information and type data
/// </summary>
public static class PokemonGameInfoService
{
    /// <summary>
    /// Get all games by generation
    /// </summary>
    public static IEnumerable<GameInfoDomain> GetGamesByGeneration(int generation)
    {
        return generation switch
        {
            1 => new[]
            {
                new GameInfoDomain(1, "Red", 1),
                new GameInfoDomain(2, "Blue", 1),
                new GameInfoDomain(3, "Yellow", 1)
            },
            2 => new[]
            {
                new GameInfoDomain(4, "Gold", 2),
                new GameInfoDomain(5, "Silver", 2),
                new GameInfoDomain(7, "Crystal", 2)
            },
            3 => new[]
            {
                new GameInfoDomain(8, "Ruby", 3),
                new GameInfoDomain(9, "Sapphire", 3),
                new GameInfoDomain(10, "Emerald", 3),
                new GameInfoDomain(11, "FireRed", 3),
                new GameInfoDomain(12, "LeafGreen", 3)
            },
            4 => new[]
            {
                new GameInfoDomain(13, "Diamond", 4),
                new GameInfoDomain(14, "Pearl", 4),
                new GameInfoDomain(15, "Platinum", 4),
                new GameInfoDomain(16, "HeartGold", 4),
                new GameInfoDomain(17, "SoulSilver", 4)
            },
            5 => new[]
            {
                new GameInfoDomain(18, "Black", 5),
                new GameInfoDomain(19, "White", 5),
                new GameInfoDomain(20, "Black2", 5),
                new GameInfoDomain(21, "White2", 5)
            },
            6 => new[]
            {
                new GameInfoDomain(24, "X", 6),
                new GameInfoDomain(25, "Y", 6),
                new GameInfoDomain(26, "OmegaRuby", 6),
                new GameInfoDomain(27, "AlphaSapphire", 6)
            },
            7 => new[]
            {
                new GameInfoDomain(30, "Sun", 7),
                new GameInfoDomain(31, "Moon", 7),
                new GameInfoDomain(32, "UltraSun", 7),
                new GameInfoDomain(33, "UltraMoon", 7)
            },
            8 => new[]
            {
                new GameInfoDomain(44, "Sword", 8),
                new GameInfoDomain(45, "Shield", 8),
                new GameInfoDomain(48, "Legends Arceus", 8),
                new GameInfoDomain(49, "Brilliant Diamond", 8),
                new GameInfoDomain(50, "Shining Pearl", 8)
            },
            9 => new[]
            {
                new GameInfoDomain(51, "Scarlet", 9),
                new GameInfoDomain(52, "Violet", 9)
            },
            _ => Array.Empty<GameInfoDomain>()
        };
    }

    /// <summary>
    /// Get species that were introduced in a specific generation
    /// </summary>
    public static IEnumerable<int> GetSpeciesInGeneration(int generation)
    {
        return generation switch
        {
            1 => Enumerable.Range(1, 151),     // Bulbasaur to Mew
            2 => Enumerable.Range(152, 100),   // Chikorita to Celebi
            3 => Enumerable.Range(252, 135),   // Treecko to Deoxys
            4 => Enumerable.Range(387, 107),   // Turtwig to Arceus
            5 => Enumerable.Range(494, 156),   // Victini to Genesect
            6 => Enumerable.Range(650, 72),    // Chespin to Volcanion
            7 => Enumerable.Range(722, 81),    // Rowlet to Marshadow
            8 => Enumerable.Range(810, 89),    // Grookey to Eternatus
            9 => Enumerable.Range(906, 112),   // Sprigatito to current
            _ => Array.Empty<int>()
        };
    }

    /// <summary>
    /// Get species IDs by name (case-insensitive search)
    /// </summary>
    public static IEnumerable<int> GetSpeciesIdsByName(string name)
    {
        var results = new List<int>();
        
        // Search through all species names
        for (int i = 1; i < PKHeX.Core.GameInfo.Strings.Species.Count; i++)
        {
            var speciesName = PkHexStringService.GetSpeciesName(i);
            if (speciesName.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(i);
            }
        }
        
        return results;
    }

    /// <summary>
    /// Get species with specific types based on filter options
    /// </summary>
    public static IEnumerable<int> GetSpeciesWithTypes(TypeFilterOptions options)
    {
        var results = new List<int>();
        
        // Get personal table for type information
        var pt = PersonalTable.SWSH; // Using Sword/Shield as default, could be parameterized
        
        for (int i = 1; i < pt.MaxSpeciesID; i++)
        {
            var personal = pt[i];
            if (personal == null) continue;
            
            var primaryType = personal.Type1;
            var secondaryType = personal.Type2;
            
            if (MatchesTypeFilter(primaryType, secondaryType, options))
            {
                results.Add(i);
            }
        }
        
        return results;
    }

    /// <summary>
    /// Get generation number from game ID
    /// </summary>
    public static int GetGenerationFromGame(int gameId)
    {
        return gameId switch
        {
            >= 1 and <= 3 => 1,       // RBY
            >= 4 and <= 7 => 2,       // GSC
            >= 8 and <= 12 => 3,      // RSE, FRLG
            >= 13 and <= 17 => 4,     // DPPt, HGSS
            >= 18 and <= 21 => 5,     // BW, B2W2
            >= 24 and <= 27 => 6,     // XY, ORAS
            >= 30 and <= 33 => 7,     // SM, USUM
            >= 44 and <= 50 => 8,     // SwSh, BDSP, LA
            >= 51 and <= 52 => 9,     // SV
            _ => 0
        };
    }

    /// <summary>
    /// Get type name by ID
    /// </summary>
    public static string GetTypeName(int typeId)
    {
        return PkHexStringService.GetTypeName(typeId);
    }

    /// <summary>
    /// Get all available types
    /// </summary>
    public static IEnumerable<(int Id, string Name)> GetAllTypes()
    {
        var types = new List<(int, string)>();
        
        for (int i = 0; i < PKHeX.Core.GameInfo.Strings.Types.Count; i++)
        {
            var typeName = PkHexStringService.GetTypeName(i);
            if (!string.IsNullOrEmpty(typeName) && typeName != "???" && !typeName.StartsWith("Type#"))
            {
                types.Add((i, typeName));
            }
        }
        
        return types;
    }

    /// <summary>
    /// Check if types match the filter criteria
    /// </summary>
    private static bool MatchesTypeFilter(int primaryType, int secondaryType, TypeFilterOptions options)
    {
        var hasSecondaryType = secondaryType != primaryType;
        
        return options.Mode switch
        {
            TypeFilterMode.HasAnyType => 
                (options.PrimaryType == null || primaryType == options.PrimaryType || 
                 (hasSecondaryType && secondaryType == options.PrimaryType)) &&
                (options.SecondaryType == null || primaryType == options.SecondaryType || 
                 (hasSecondaryType && secondaryType == options.SecondaryType)),
                 
            TypeFilterMode.HasAllTypes =>
                (options.PrimaryType == null || primaryType == options.PrimaryType || 
                 (hasSecondaryType && secondaryType == options.PrimaryType)) &&
                (options.SecondaryType == null || primaryType == options.SecondaryType || 
                 (hasSecondaryType && secondaryType == options.SecondaryType)),
                 
            TypeFilterMode.HasOnlyTypes =>
                (!hasSecondaryType && options.PrimaryType == primaryType && options.SecondaryType == null) ||
                (hasSecondaryType && 
                 ((primaryType == options.PrimaryType && secondaryType == options.SecondaryType) ||
                  (!options.EnforceTypeOrder && primaryType == options.SecondaryType && secondaryType == options.PrimaryType))),
                  
            TypeFilterMode.PrimaryTypeOnly =>
                !hasSecondaryType && options.PrimaryType == primaryType,
                
            TypeFilterMode.ExactTypeOrder =>
                hasSecondaryType && primaryType == options.PrimaryType && secondaryType == options.SecondaryType,
                
            TypeFilterMode.BothTypesAnyOrder =>
                hasSecondaryType && 
                ((primaryType == options.PrimaryType && secondaryType == options.SecondaryType) ||
                 (primaryType == options.SecondaryType && secondaryType == options.PrimaryType)),
                 
            _ => false
        };
    }
}
