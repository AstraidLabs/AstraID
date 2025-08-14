using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user account is locked/deactivated.
/// </summary>
public sealed record UserLocked(Guid UserId, string Reason) : DomainEventBase;
