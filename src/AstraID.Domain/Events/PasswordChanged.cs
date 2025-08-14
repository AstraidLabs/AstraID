using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a user changes their password.
/// </summary>
public sealed record PasswordChanged(Guid UserId) : DomainEventBase;
