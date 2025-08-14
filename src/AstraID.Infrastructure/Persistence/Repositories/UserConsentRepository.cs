using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserConsentRepository"/>.
/// </summary>
public class UserConsentRepository : IUserConsentRepository
{
    private readonly AstraIdDbContext _db;

    public UserConsentRepository(AstraIdDbContext db) => _db = db;

    public Task<UserConsent?> GetByUserAndClientAsync(Guid userId, string clientId, CancellationToken ct = default) =>
        _db.UserConsents.FirstOrDefaultAsync(c => c.UserId == userId && c.ClientId == clientId, ct);

    public Task AddAsync(UserConsent consent, CancellationToken ct = default) =>
        _db.UserConsents.AddAsync(consent, ct).AsTask();
}
