using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="AppRole"/> aggregates.</summary>
public interface IRoleRepository
{
    /// <summary>Gets a role by identifier.</summary>
    Task<AppRole?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Gets a role by name.</summary>
    Task<AppRole?> GetByNameAsync(string normalizedName, CancellationToken ct = default);

    /// <summary>Adds a new role.</summary>
    Task AddAsync(AppRole role, CancellationToken ct = default);
}
