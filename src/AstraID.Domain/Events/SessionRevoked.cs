using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user session is revoked.
/// </summary>
public sealed record SessionRevoked(Guid UserId, Guid SessionId, string Reason) : DomainEventBase;
