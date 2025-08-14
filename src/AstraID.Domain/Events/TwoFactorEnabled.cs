using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when two-factor authentication is enabled for a user.
/// </summary>
public sealed record TwoFactorEnabled(Guid UserId) : DomainEventBase;
