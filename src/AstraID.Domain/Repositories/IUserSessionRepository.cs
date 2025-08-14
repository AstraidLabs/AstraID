using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="UserSession"/> aggregates.</summary>
public interface IUserSessionRepository
{
    /// <summary>Gets an active session by device.</summary>
    Task<UserSession?> GetActiveByDeviceAsync(Guid userId, string deviceId, CancellationToken ct = default);

    /// <summary>Adds a new session.</summary>
    Task AddAsync(UserSession session, CancellationToken ct = default);

    /// <summary>Revokes all sessions for a user.</summary>
    Task<int> RevokeAllAsync(Guid userId, string reason, DateTime utcNow, CancellationToken ct = default);
}
