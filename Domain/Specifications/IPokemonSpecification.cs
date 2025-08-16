using BeastVault.Api.Domain.Entities;

namespace BeastVault.Api.Domain.Specifications;

/// <summary>
/// Base interface for Pokemon specifications (filters)
/// </summary>
public interface IPokemonSpecification
{
    /// <summary>
    /// Apply the specification to a Pokemon query
    /// </summary>
    IQueryable<PokemonEntity> Apply(IQueryable<PokemonEntity> query);
}

/// <summary>
/// Composite specification that combines multiple specifications
/// </summary>
public class CompositeSpecification : IPokemonSpecification
{
    private readonly IEnumerable<IPokemonSpecification> _specifications;

    public CompositeSpecification(IEnumerable<IPokemonSpecification> specifications)
    {
        _specifications = specifications;
    }

    public IQueryable<PokemonEntity> Apply(IQueryable<PokemonEntity> query)
    {
        return _specifications.Aggregate(query, (current, spec) => spec.Apply(current));
    }
}
