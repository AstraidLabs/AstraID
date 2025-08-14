using System.Linq;
using AstraID.Domain.Primitives;

namespace AstraID.Domain.Entities;

/// <summary>
/// Keeps track of previous password hashes to prevent reuse.
/// </summary>
public sealed class PasswordHistory : Entity<Guid>
{
    /// <summary>
    /// User owning the password history entry.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Stored password hash.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when the password was changed.
    /// </summary>
    public DateTime ChangedUtc { get; private set; }

    private PasswordHistory()
    {
    }

    private PasswordHistory(Guid userId, string passwordHash) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        UserId = userId;
        PasswordHash = passwordHash;
        ChangedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a password hash for the user.
    /// </summary>
    public static PasswordHistory Record(Guid userId, string passwordHash) => new(userId, passwordHash);

    /// <summary>
    /// Checks if the provided hash matches this history entry.
    /// </summary>
    public bool Matches(string hash) => string.Equals(PasswordHash, hash, StringComparison.Ordinal);

    /// <summary>
    /// Ensures the new hash has not been used previously.
    /// </summary>
    public static void EnsureNotReused(IEnumerable<PasswordHistory> history, string newHash)
    {
        if (history.Any(h => h.Matches(newHash)))
            throw new InvalidOperationException("Password reuse is not allowed.");
    }
}
