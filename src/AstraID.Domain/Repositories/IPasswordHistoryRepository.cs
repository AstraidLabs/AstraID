using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="PasswordHistory"/> entries.</summary>
public interface IPasswordHistoryRepository
{
    /// <summary>Gets recent password hashes for the user.</summary>
    Task<IReadOnlyList<string>> GetRecentHashesAsync(Guid userId, int take, CancellationToken ct = default);

    /// <summary>Adds a password history entry.</summary>
    Task AddAsync(PasswordHistory history, CancellationToken ct = default);
}
