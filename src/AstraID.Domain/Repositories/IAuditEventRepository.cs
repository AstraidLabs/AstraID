using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="AuditEvent"/> aggregates.</summary>
public interface IAuditEventRepository
{
    /// <summary>Adds a new audit event.</summary>
    Task AddAsync(AuditEvent auditEvent, CancellationToken ct = default);
}
