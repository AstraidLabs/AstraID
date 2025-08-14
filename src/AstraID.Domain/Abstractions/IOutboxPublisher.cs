namespace AstraID.Domain.Abstractions;

/// <summary>
/// Publishes pending outbox messages.
/// </summary>
public interface IOutboxPublisher
{
    /// <summary>
    /// Processes and dispatches pending outbox messages.
    /// </summary>
    Task PublishPendingAsync(CancellationToken ct = default);
}
