using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="Client"/> aggregates.</summary>
public interface IClientRepository
{
    /// <summary>Gets a client by client identifier.</summary>
    Task<Client?> GetByClientIdAsync(string clientId, Guid? tenantId, CancellationToken ct = default);

    /// <summary>Adds a new client.</summary>
    Task AddAsync(Client client, CancellationToken ct = default);
}
