namespace AstraID.Domain.Primitives;

/// <summary>
/// Represents a domain event with occurrence timestamp.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the UTC time when the event occurred.
    /// </summary>
    DateTime OccurredUtc { get; }
}
