namespace AstraID.Domain.Primitives;

/// <summary>
/// Base class for aggregate roots with domain event support.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets domain events raised by the aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Gets the concurrency stamp for optimistic concurrency checks.
    /// </summary>
    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString("N");

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Updates the concurrency stamp.
    /// </summary>
    protected void Touch() => ConcurrencyStamp = Guid.NewGuid().ToString("N");
}
