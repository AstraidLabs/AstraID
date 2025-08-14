using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="Permission"/> aggregates.</summary>
public interface IPermissionRepository
{
    /// <summary>Gets a permission by identifier.</summary>
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Adds a new permission.</summary>
    Task AddAsync(Permission permission, CancellationToken ct = default);
}
