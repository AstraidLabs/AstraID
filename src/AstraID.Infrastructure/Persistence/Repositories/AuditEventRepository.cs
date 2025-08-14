using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuditEventRepository"/>.
/// </summary>
public class AuditEventRepository : IAuditEventRepository
{
    private readonly AstraIdDbContext _db;

    public AuditEventRepository(AstraIdDbContext db) => _db = db;

    public Task AddAsync(AuditEvent auditEvent, CancellationToken ct = default) =>
        _db.AuditEvents.AddAsync(auditEvent, ct).AsTask();
}
