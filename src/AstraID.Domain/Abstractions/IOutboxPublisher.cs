namespace AstraID.Domain.Abstractions;

/// <summary>Enqueues outbox messages for later publishing.</summary>
public interface IOutboxPublisher
{
    /// <summary>Enqueue domain/integration event to outbox.</summary>
    Task EnqueueAsync(IOutboxMessage message, CancellationToken ct = default);
}
