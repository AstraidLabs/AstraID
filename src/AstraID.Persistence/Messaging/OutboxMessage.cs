using AstraID.Domain.Abstractions;

namespace AstraID.Persistence.Messaging;

/// <summary>
/// Outbox message stored for later processing.
/// </summary>
public class OutboxMessage : IOutboxMessage
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public int Attempts { get; set; }
}
