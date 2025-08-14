using System.Text.Json;
using System.Linq;
using AstraID.Domain.Primitives;
using AstraID.Infrastructure.Messaging;
using AstraID.Persistence;
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
        if (eventData.Context is not AstraIdDbContext db)
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
                    Type = domainEvent.GetType().FullName ?? string.Empty,
                    PayloadJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    CreatedUtc = DateTime.UtcNow
                };
                db.OutboxMessages.Add(message);
            }
            entry.Entity.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
