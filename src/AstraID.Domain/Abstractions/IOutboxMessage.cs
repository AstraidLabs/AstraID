namespace AstraID.Domain.Abstractions;

/// <summary>Represents an outbox message for integration events.</summary>
public interface IOutboxMessage
{
    Guid Id { get; }
    DateTime CreatedUtc { get; }
    string Type { get; }
    string PayloadJson { get; }
    string? CorrelationId { get; }
    DateTime? ProcessedUtc { get; }
    int Attempts { get; }
}
