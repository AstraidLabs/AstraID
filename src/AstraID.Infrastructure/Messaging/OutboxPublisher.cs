using AstraID.Domain.Abstractions;
using AstraID.Persistence;

namespace AstraID.Infrastructure.Messaging;

/// <summary>
/// Persists outbox messages in the database.
/// </summary>
public class OutboxPublisher : IOutboxPublisher
{
    private readonly AstraIdDbContext _db;

    public OutboxPublisher(AstraIdDbContext db) => _db = db;

    public Task EnqueueAsync(IOutboxMessage message, CancellationToken ct = default)
    {
        _db.OutboxMessages.Add((OutboxMessage)message);
        return Task.CompletedTask;
    }
}
