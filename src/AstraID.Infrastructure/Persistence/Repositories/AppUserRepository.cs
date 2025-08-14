using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAppUserRepository"/>.
/// </summary>
public class AppUserRepository : IAppUserRepository
{
    private readonly AstraIdDbContext _db;

    public AppUserRepository(AstraIdDbContext db) => _db = db;

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FindAsync(new object?[] { id }, ct).AsTask();

    public Task<AppUser?> GetByEmailAsync(string normalizedEmail, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail.ToUpperInvariant(), ct);

    public Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken ct = default) =>
        _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail.ToUpperInvariant(), ct);

    public Task AddAsync(AppUser user, CancellationToken ct = default) =>
        _db.Users.AddAsync(user, ct).AsTask();
}
