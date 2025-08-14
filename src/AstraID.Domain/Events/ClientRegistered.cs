using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a new client is registered.
/// </summary>
public sealed record ClientRegistered(Guid ClientId, string ClientIdentifier) : DomainEventBase;
