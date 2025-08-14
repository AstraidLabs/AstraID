using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Entities;

/// <summary>
/// Historical record of client secrets.
/// </summary>
public sealed class ClientSecretHistory : Entity<Guid>
{
    /// <summary>
    /// Owning client identifier.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Secret hash value.
    /// </summary>
    public HashedSecret SecretHash { get; private set; } = null!;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// Indicates whether this secret is currently active.
    /// </summary>
    public bool Active { get; private set; }

    private ClientSecretHistory()
    {
    }

    private ClientSecretHistory(Guid clientId, HashedSecret hash, bool active) : base(Guid.NewGuid())
    {
        ClientId = clientId;
        SecretHash = hash;
        CreatedUtc = DateTime.UtcNow;
        Active = active;
    }

    /// <summary>
    /// Creates a new history entry.
    /// </summary>
    public static ClientSecretHistory Create(Guid clientId, HashedSecret hash, bool active = true)
        => new(clientId, hash, active);

    /// <summary>
    /// Marks the secret as inactive.
    /// </summary>
    public void Deactivate() => Active = false;
}
