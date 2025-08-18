using System.Collections.Generic;
using System.Linq;
using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserSessionRepository"/>.
/// </summary>
public class UserSessionRepository : IUserSessionRepository
{
    private readonly AstraIdDbContext _db;

    public UserSessionRepository(AstraIdDbContext db) => _db = db;

    public Task<UserSession?> GetActiveByDeviceAsync(Guid userId, string deviceId, CancellationToken ct = default) =>
        _db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == userId && s.DeviceId.Value == deviceId && s.RevokedUtc == null,
            ct);

    public Task AddAsync(UserSession session, CancellationToken ct = default) =>
        _db.UserSessions.AddAsync(session, ct).AsTask();

    public Task<int> RevokeAllAsync(Guid userId, string reason, DateTime utcNow, CancellationToken ct = default) =>
        _db.UserSessions.Where(s => s.UserId == userId && s.RevokedUtc == null)
            .ExecuteUpdateAsync(up => up.SetProperty(s => s.RevokedUtc, utcNow)
                                        .SetProperty(s => s.RevokeReason, reason), ct);

    public async Task<IReadOnlyList<UserSession>> ListActiveByUserAsync(Guid userId, CancellationToken ct = default) =>
        await _db.UserSessions.Where(s => s.UserId == userId && s.RevokedUtc == null).ToListAsync(ct);
}
