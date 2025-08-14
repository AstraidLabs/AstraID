using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a new user registers.
/// </summary>
public sealed record UserRegistered(Guid UserId, string Email) : DomainEventBase;
