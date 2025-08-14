namespace AstraID.Domain.Primitives;

/// <summary>
/// Base type for domain events providing the occurred timestamp.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredUtc { get; } = DateTime.UtcNow;
}
