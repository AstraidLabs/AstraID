using AstraID.Domain.Primitives;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents a tenant in a multi-tenant deployment.
/// </summary>
public sealed class Tenant : AggregateRoot<Guid>
{
    /// <summary>
    /// Tenant name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Indicates whether the tenant is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    private Tenant()
    {
    }

    private Tenant(string name) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length is < 1 or > 128)
            throw new ArgumentException("Tenant name must be between 1 and 128 characters.", nameof(name));

        Name = name.Trim();
    }

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    public static Tenant Create(string name) => new(name);

    /// <summary>
    /// Activates the tenant.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    /// <summary>
    /// Deactivates the tenant. TODO: ensure no active users are orphaned.
    /// </summary>
    public void Deactivate()
    {
        // TODO: Validate no active users are orphaned.
        IsActive = false;
        Touch();
    }
}
