using AstraID.Domain.Primitives;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents a one-time recovery code.
/// </summary>
public sealed class RecoveryCode : Entity<Guid>
{
    /// <summary>
    /// User owning the code.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Hashed representation of the recovery code.
    /// </summary>
    public string CodeHash { get; private set; } = string.Empty;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// Timestamp when the code was used.
    /// </summary>
    public DateTime? UsedUtc { get; private set; }

    /// <summary>
    /// Indicates whether the code has been used.
    /// </summary>
    public bool Used => UsedUtc != null;

    private RecoveryCode()
    {
    }

    private RecoveryCode(Guid userId, string codeHash) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(codeHash))
            throw new ArgumentException("Code hash cannot be empty.", nameof(codeHash));

        UserId = userId;
        CodeHash = codeHash;
        CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Issues a new recovery code.
    /// </summary>
    public static RecoveryCode Issue(Guid userId, string codeHash) => new(userId, codeHash);

    /// <summary>
    /// Marks the recovery code as used. Can only be done once.
    /// </summary>
    public void MarkUsed()
    {
        if (Used)
            throw new InvalidOperationException("Recovery code already used.");

        UsedUtc = DateTime.UtcNow;
    }
}
