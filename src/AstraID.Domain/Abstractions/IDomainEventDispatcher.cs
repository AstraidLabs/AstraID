using AstraID.Domain.Primitives;

namespace AstraID.Domain.Abstractions;

/// <summary>Dispatches domain events raised by aggregates.</summary>
public interface IDomainEventDispatcher
{
    /// <summary>Dispatches domain events raised by aggregates.</summary>
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
