using System.Text.Json;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Primitives;
using AstraID.Persistence.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AstraID.Infrastructure.Messaging;

/// <summary>
/// Publishes pending outbox messages by dispatching their domain events.
/// </summary>
public sealed class OutboxPublisher : IOutboxPublisher
{
    private readonly DbContext _db;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(
        DbContext db,
        IDomainEventDispatcher dispatcher,
        ILogger<OutboxPublisher> logger)
    {
        _db = db;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishPendingAsync(CancellationToken ct = default)
    {
        var messages = await _db.Set<OutboxMessage>()
            .Where(m => m.ProcessedUtc == null)
            .OrderBy(m => m.CreatedUtc)
            .Take(100)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type == null || !typeof(IDomainEvent).IsAssignableFrom(type))
                {
                    _logger.LogWarning("Unknown domain event type {Type}", message.Type);
                    message.ProcessedUtc = DateTime.UtcNow;
                    continue;
                }

                var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(
                    message.PayloadJson,
                    type,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (domainEvent is null)
                {
                    message.ProcessedUtc = DateTime.UtcNow;
                    continue;
                }

                await _dispatcher.DispatchAsync(new[] { domainEvent }, ct).ConfigureAwait(false);
                message.ProcessedUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                message.Attempts++;
                _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
            }
        }

        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
