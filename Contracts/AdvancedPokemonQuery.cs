using BeastVault.Api.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace BeastVault.Api.Contracts;

/// <summary>
/// Advanced query parameters for filtering and sorting Pokemon
/// </summary>
public record AdvancedPokemonQuery
{
    #region Basic Filters
    
    /// <summary>
    /// Text search across nickname, OT name, and notes
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Filter by specific Pokedex number (Species ID)
    /// </summary>
    public int? PokedexNumber { get; init; }

    /// <summary>
    /// Filter by species name (partial match)
    /// </summary>
    public string? SpeciesName { get; init; }

    /// <summary>
    /// Filter by nickname (partial match)
    /// </summary>
    public string? Nickname { get; init; }

    /// <summary>
    /// Filter by shiny status
    /// </summary>
    public bool? IsShiny { get; init; }

    /// <summary>
    /// Filter by form ID
    /// </summary>
    public int? Form { get; init; }

    /// <summary>
    /// Filter by gender (0 = undefined, 1 = male, 2 = female)
    /// </summary>
    public int? Gender { get; init; }

    #endregion

    #region Generation Filters

    /// <summary>
    /// Filter by origin generation (game where Pokemon originated)
    /// </summary>
    public int? OriginGeneration { get; init; }

    /// <summary>
    /// Filter by captured generation (generation where species was introduced)
    /// </summary>
    public int? CapturedGeneration { get; init; }

    #endregion

    #region Equipment Filters

    /// <summary>
    /// Filter by Pokeball ID
    /// </summary>
    public int? PokeballId { get; init; }

    /// <summary>
    /// Filter by held item ID
    /// </summary>
    public int? HeldItemId { get; init; }

    #endregion

    #region Type Filters

    /// <summary>
    /// Primary type ID for type filtering
    /// </summary>
    public int? PrimaryType { get; init; }

    /// <summary>
    /// Secondary type ID for type filtering
    /// </summary>
    public int? SecondaryType { get; init; }

    /// <summary>
    /// Type filter mode (how to apply type filters)
    /// </summary>
    public TypeFilterMode TypeFilterMode { get; init; } = TypeFilterMode.HasAnyType;

    /// <summary>
    /// Whether to enforce exact type order for dual-type filtering
    /// </summary>
    public bool EnforceTypeOrder { get; init; } = false;

    #endregion

    #region Level and Stats Filters

    /// <summary>
    /// Minimum level filter
    /// </summary>
    [Range(1, 100)]
    public int? MinLevel { get; init; }

    /// <summary>
    /// Maximum level filter
    /// </summary>
    [Range(1, 100)]
    public int? MaxLevel { get; init; }

    #endregion

    #region Sorting

    /// <summary>
    /// Primary sort field
    /// </summary>
    public PokemonSortField SortBy { get; init; } = PokemonSortField.Id;

    /// <summary>
    /// Primary sort direction
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;

    /// <summary>
    /// Secondary sort field (optional)
    /// </summary>
    public PokemonSortField? ThenSortBy { get; init; }

    /// <summary>
    /// Secondary sort direction
    /// </summary>
    public SortDirection ThenSortDirection { get; init; } = SortDirection.Ascending;

    #endregion

    #region Pagination

    /// <summary>
    /// Number of items to skip (for pagination)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Number of items to take (max recommended: 100)
    /// </summary>
    [Range(1, 500)]
    public int Take { get; init; } = 50;

    #endregion

    #region Legacy Support

    /// <summary>
    /// Legacy species ID filter (use PokedexNumber instead)
    /// </summary>
    [Obsolete("Use PokedexNumber instead")]
    public int? SpeciesId { get; init; }

    /// <summary>
    /// Legacy ball ID filter (use PokeballId instead)
    /// </summary>
    [Obsolete("Use PokeballId instead")]
    public int? BallId { get; init; }

    /// <summary>
    /// Legacy origin game filter
    /// </summary>
    public int? OriginGame { get; init; }

    /// <summary>
    /// Legacy tera type filter
    /// </summary>
    public int? TeraType { get; init; }

    #endregion

    /// <summary>
    /// Convert to type filter options
    /// </summary>
    public TypeFilterOptions? ToTypeFilterOptions()
    {
        if (PrimaryType == null && SecondaryType == null)
            return null;

        return new TypeFilterOptions
        {
            PrimaryType = PrimaryType,
            SecondaryType = SecondaryType,
            Mode = TypeFilterMode,
            EnforceTypeOrder = EnforceTypeOrder
        };
    }

    /// <summary>
    /// Get primary sort options
    /// </summary>
    public PokemonSortOptions GetPrimarySortOptions()
    {
        return new PokemonSortOptions
        {
            SortBy = SortBy,
            Direction = SortDirection
        };
    }

    /// <summary>
    /// Get secondary sort options (if specified)
    /// </summary>
    public PokemonSortOptions? GetSecondarySortOptions()
    {
        if (ThenSortBy == null)
            return null;

        return new PokemonSortOptions
        {
            SortBy = ThenSortBy.Value,
            Direction = ThenSortDirection
        };
    }

    /// <summary>
    /// Get all sort options as a list
    /// </summary>
    public IEnumerable<PokemonSortOptions> GetAllSortOptions()
    {
        yield return GetPrimarySortOptions();
        
        var secondary = GetSecondarySortOptions();
        if (secondary != null)
            yield return secondary;
    }
}
