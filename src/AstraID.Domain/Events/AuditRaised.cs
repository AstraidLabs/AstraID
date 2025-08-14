using AstraID.Domain.Primitives;

namespace AstraID.Domain.Events;

/// <summary>
/// Raised when an audit event is created.
/// </summary>
public sealed record AuditRaised(Guid? UserId, string EventType) : DomainEventBase;
