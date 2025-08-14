using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a password reset is requested for a user.
/// </summary>
public sealed record PasswordResetRequested(Guid UserId) : DomainEventBase;
