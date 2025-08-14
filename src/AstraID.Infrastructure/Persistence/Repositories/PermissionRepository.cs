using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPermissionRepository"/>.
/// </summary>
public class PermissionRepository : IPermissionRepository
{
    private readonly AstraIdDbContext _db;

    public PermissionRepository(AstraIdDbContext db) => _db = db;

    public Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Permissions.FindAsync(new object?[] { id }, ct).AsTask();

    public Task AddAsync(Permission permission, CancellationToken ct = default) =>
        _db.Permissions.AddAsync(permission, ct).AsTask();
}
