using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user session starts.
/// </summary>
public sealed record SessionStarted(Guid UserId, Guid SessionId) : DomainEventBase;
