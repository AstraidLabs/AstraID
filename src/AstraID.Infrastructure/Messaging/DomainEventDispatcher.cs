using System.Collections.Generic;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Primitives;

namespace AstraID.Infrastructure.Messaging;

/// <summary>
/// Simple domain event dispatcher that resolves handlers from DI and invokes them.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider) => _provider = provider;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        // Resolve handlers of type IDomainEventHandler<T> if present
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = (IEnumerable<object>)(_provider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType)) ?? Array.Empty<object>());
            foreach (var handler in handlers)
            {
                // Reflection keeps dispatcher decoupled from handler types at the cost of some overhead.
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                    await (Task)method.Invoke(handler, new object?[] { domainEvent, ct })!;
            }
        }
    }
}

/// <summary>
/// Handler contract for domain events.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
