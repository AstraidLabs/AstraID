using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user's consent is revoked.
/// </summary>
public sealed record ConsentRevoked(Guid UserId, string ClientId) : DomainEventBase;
