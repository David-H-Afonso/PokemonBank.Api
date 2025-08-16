namespace BeastVault.Api.Domain.ValueObjects;

/// <summary>
/// Options for advanced type filtering
/// </summary>
public record TypeFilterOptions
{
    /// <summary>
    /// Primary type ID to filter by
    /// </summary>
    public int? PrimaryType { get; init; }

    /// <summary>
    /// Secondary type ID to filter by
    /// </summary>
    public int? SecondaryType { get; init; }

    /// <summary>
    /// Type filter mode
    /// </summary>
    public TypeFilterMode Mode { get; init; } = TypeFilterMode.HasAnyType;

    /// <summary>
    /// Whether to enforce type order (for dual-type filtering)
    /// </summary>
    public bool EnforceTypeOrder { get; init; } = false;
}

/// <summary>
/// Type filtering modes
/// </summary>
public enum TypeFilterMode
{
    /// <summary>
    /// Pokemon has any of the specified types (OR operation)
    /// </summary>
    HasAnyType,

    /// <summary>
    /// Pokemon has all specified types (AND operation)
    /// </summary>
    HasAllTypes,

    /// <summary>
    /// Pokemon has only the specified type(s) and no others
    /// </summary>
    HasOnlyTypes,

    /// <summary>
    /// Pokemon has the primary type only (no secondary type)
    /// </summary>
    PrimaryTypeOnly,

    /// <summary>
    /// Pokemon has both primary and secondary types in exact order
    /// </summary>
    ExactTypeOrder,

    /// <summary>
    /// Pokemon has both primary and secondary types in any order
    /// </summary>
    BothTypesAnyOrder
}

/// <summary>
/// Sorting options for Pokemon queries
/// </summary>
public record PokemonSortOptions
{
    /// <summary>
    /// Field to sort by
    /// </summary>
    public PokemonSortField SortBy { get; init; } = PokemonSortField.Id;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection Direction { get; init; } = SortDirection.Descending;
}

/// <summary>
/// Available fields for Pokemon sorting
/// </summary>
public enum PokemonSortField
{
    Id,
    PokedexNumber,
    SpeciesName,
    Nickname,
    Level,
    OriginGeneration,
    CapturedGeneration,
    Pokeball,
    Gender,
    IsShiny,
    Form,
    CreatedAt,
    Favorite
}

/// <summary>
/// Sort direction
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Game information for generation filtering
/// </summary>
public record GameInfo(int GameId, string Name, int Generation);

/// <summary>
/// Species type information
/// </summary>
public record SpeciesTypeInfo(int SpeciesId, int PrimaryType, int? SecondaryType);
