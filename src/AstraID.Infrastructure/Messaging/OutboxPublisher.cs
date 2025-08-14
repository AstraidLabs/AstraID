using System.Text.Json;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Primitives;
using AstraID.Persistence.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AstraID.Infrastructure.Messaging;

/// <summary>
/// Publishes pending outbox messages by dispatching their domain events.
/// </summary>
public sealed class OutboxPublisher : IOutboxPublisher
{
    private readonly DbContext _db;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly IOutboxUnknownMessageHandler? _unknownHandler;

    private static readonly IReadOnlyDictionary<string, Type> _eventTypes =
        typeof(IDomainEvent).Assembly
            .GetTypes()
            .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.AssemblyQualifiedName!, t => t);

    public OutboxPublisher(
        DbContext db,
        IDomainEventDispatcher dispatcher,
        ILogger<OutboxPublisher> logger,
        IOutboxUnknownMessageHandler? unknownHandler = null)
    {
        _db = db;
        _dispatcher = dispatcher;
        _logger = logger;
        _unknownHandler = unknownHandler;
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
                if (!_eventTypes.TryGetValue(message.Type, out var type))
                {
                    _logger.LogWarning("Unknown domain event type {Type}", message.Type);
                    if (_unknownHandler is not null)
                        await _unknownHandler.HandleAsync(message, ct).ConfigureAwait(false);
                    message.ProcessedUtc = DateTime.UtcNow;
                    continue;
                }

                var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(
                    message.PayloadJson,
                    type,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (domainEvent is null)
                {
                    _logger.LogWarning("Failed to deserialize domain event {Type}", message.Type);
                    if (_unknownHandler is not null)
                        await _unknownHandler.HandleAsync(message, ct).ConfigureAwait(false);
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

public interface IOutboxUnknownMessageHandler
{
    Task HandleAsync(OutboxMessage message, CancellationToken ct = default);
}
