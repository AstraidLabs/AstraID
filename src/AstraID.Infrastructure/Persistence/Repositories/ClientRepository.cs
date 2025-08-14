using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IClientRepository"/>.
/// </summary>
public class ClientRepository : IClientRepository
{
    private readonly AstraIdDbContext _db;

    public ClientRepository(AstraIdDbContext db) => _db = db;

    public Task<Client?> GetByClientIdAsync(string clientId, Guid? tenantId, CancellationToken ct = default) =>
        _db.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId && c.TenantId == tenantId, ct);

    public Task AddAsync(Client client, CancellationToken ct = default) =>
        _db.Clients.AddAsync(client, ct).AsTask();
}
