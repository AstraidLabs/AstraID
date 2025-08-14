using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="AppUser"/> aggregates.</summary>
public interface IAppUserRepository
{
    /// <summary>Gets a user by identifier.</summary>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Gets a user by normalized email.</summary>
    Task<AppUser?> GetByEmailAsync(string normalizedEmail, CancellationToken ct = default);

    /// <summary>Checks if a user exists by normalized email.</summary>
    Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken ct = default);

    /// <summary>Adds a new user.</summary>
    Task AddAsync(AppUser user, CancellationToken ct = default);
}
