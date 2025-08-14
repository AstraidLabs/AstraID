using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user grants consent to a client.
/// </summary>
public sealed record ConsentGranted(Guid UserId, string ClientId, string[] Scopes) : DomainEventBase;
