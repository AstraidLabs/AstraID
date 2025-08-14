using System.Text.Json;
using System.Linq;
using AstraID.Domain.Primitives;
using AstraID.Persistence.Messaging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Collects domain events from aggregates and stores them as outbox messages.
/// </summary>
public sealed class DomainEventsCollectorInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not DbContext db)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var aggregates = db.ChangeTracker.Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        foreach (var entry in aggregates)
        {
            foreach (var domainEvent in entry.Entity.DomainEvents)
            {
                var message = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().AssemblyQualifiedName ?? string.Empty,
                    PayloadJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)),
                    CreatedUtc = DateTime.UtcNow,
                    Attempts = 0
                };
                db.Set<OutboxMessage>().Add(message);
            }
            entry.Entity.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
