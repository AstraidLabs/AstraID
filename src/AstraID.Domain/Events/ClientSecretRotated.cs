using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when a client's secret is rotated.
/// </summary>
public sealed record ClientSecretRotated(Guid ClientId) : DomainEventBase;
