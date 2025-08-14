using AstraID.Domain.Entities;

namespace AstraID.Domain.Services;

/// <summary>Factory helpers for creating <see cref="AuditEvent"/> instances.</summary>
public sealed class AuditDomainService
{
    /// <summary>Creates a new audit event.</summary>
    public AuditEvent Create(Guid? tenantId, Guid? userId, string? clientId, AuditEventType type, object? data = null, string? failure = null, string? correlation = null, string? ip = null, string? agent = null, string? resourceId = null, string? severity = "Info")
        => AuditEvent.From(tenantId, userId, clientId, type, data, failure, correlation, ip, agent, resourceId, severity);
}
