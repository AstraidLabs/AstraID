using AstraID.Domain.Constants;
using AstraID.Domain.Repositories;

namespace AstraID.Domain.Policies;

/// <summary>Ensures passwords are not reused.</summary>
public sealed class PasswordReusePolicy
{
    private readonly IPasswordHistoryRepository _historyRepository;

    /// <summary>Initializes the policy.</summary>
    public PasswordReusePolicy(IPasswordHistoryRepository historyRepository)
        => _historyRepository = historyRepository;

    /// <summary>Determines whether the provided password hash is allowed.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the hash was used recently.</exception>
    public async Task<bool> IsAllowedAsync(Guid userId, string newPasswordHash, CancellationToken ct = default)
    {
        var hashes = await _historyRepository.GetRecentHashesAsync(userId, SecurityConstants.PasswordHistoryDepth, ct);
        if (hashes.Contains(newPasswordHash, StringComparer.Ordinal))
            throw new InvalidOperationException("Password reuse is not allowed.");
        return true;
    }
}
