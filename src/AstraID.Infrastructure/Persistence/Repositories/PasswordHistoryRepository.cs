using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPasswordHistoryRepository"/>.
/// </summary>
public class PasswordHistoryRepository : IPasswordHistoryRepository
{
    private readonly AstraIdDbContext _db;

    public PasswordHistoryRepository(AstraIdDbContext db) => _db = db;

    public async Task<IReadOnlyList<string>> GetRecentHashesAsync(Guid userId, int take, CancellationToken ct = default) =>
        await _db.PasswordHistory
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedUtc)
            .Select(p => p.PasswordHash)
            .Take(take)
            .ToListAsync(ct);

    public Task AddAsync(PasswordHistory history, CancellationToken ct = default) =>
        _db.PasswordHistory.AddAsync(history, ct).AsTask();
}
