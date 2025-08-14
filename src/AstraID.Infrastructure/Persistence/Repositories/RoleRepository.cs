using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRoleRepository"/>.
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly AstraIdDbContext _db;

    public RoleRepository(AstraIdDbContext db) => _db = db;

    public Task<AppRole?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Roles.FindAsync(new object?[] { id }, ct).AsTask();

    public Task<AppRole?> GetByNameAsync(string normalizedName, CancellationToken ct = default) =>
        _db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, ct);

    public Task AddAsync(AppRole role, CancellationToken ct = default) =>
        _db.Roles.AddAsync(role, ct).AsTask();
}
