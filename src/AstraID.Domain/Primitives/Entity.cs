namespace AstraID.Domain.Primitives;

/// <summary>
/// Base class for entities with a typed identifier.
/// </summary>
public abstract class Entity<TId>
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    protected Entity() { }

    protected Entity(TId id) => Id = id;
}
