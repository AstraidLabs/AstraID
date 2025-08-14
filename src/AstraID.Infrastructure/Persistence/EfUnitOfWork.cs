using AstraID.Domain.Abstractions;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation over <see cref="AstraIdDbContext"/>.
/// </summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly AstraIdDbContext _db;

    public EfUnitOfWork(AstraIdDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
