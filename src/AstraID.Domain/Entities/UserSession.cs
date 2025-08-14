using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents a persisted user session/device.
/// </summary>
public sealed class UserSession : AggregateRoot<Guid>
{
    /// <summary>
    /// User owning the session.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Optional tenant identifier.
    /// </summary>
    public Guid? TenantId { get; private set; }

    /// <summary>
    /// Free-text device description.
    /// </summary>
    public string? Device { get; private set; }

    /// <summary>
    /// Device identifier value object.
    /// </summary>
    public DeviceId DeviceId { get; private set; } = null!;

    /// <summary>
    /// IP address of the session.
    /// </summary>
    public IpAddress Ip { get; private set; } = null!;

    /// <summary>
    /// User agent string.
    /// </summary>
    public UserAgent Agent { get; private set; } = null!;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// Last seen timestamp.
    /// </summary>
    public DateTime LastSeenUtc { get; private set; }

    /// <summary>
    /// Revocation timestamp if revoked.
    /// </summary>
    public DateTime? RevokedUtc { get; private set; }

    /// <summary>
    /// Reason for revocation.
    /// </summary>
    public string? RevokeReason { get; private set; }

    /// <summary>
    /// Indicates whether the session is active.
    /// </summary>
    public bool IsActive => RevokedUtc == null;

    private UserSession()
    {
    }

    private UserSession(Guid userId, DeviceId deviceId, IpAddress ip, UserAgent agent, Guid? tenantId) : base(Guid.NewGuid())
    {
        UserId = userId;
        TenantId = tenantId;
        DeviceId = deviceId;
        Ip = ip;
        Agent = agent;
        CreatedUtc = LastSeenUtc = DateTime.UtcNow;
        Raise(new SessionStarted(userId, Id));
    }

    /// <summary>
    /// Starts a new session and raises <see cref="SessionStarted"/>.
    /// </summary>
    public static UserSession Start(Guid userId, DeviceId deviceId, IpAddress ip, UserAgent agent, Guid? tenantId = null)
        => new(userId, deviceId, ip, agent, tenantId);

    /// <summary>
    /// Updates the last seen timestamp for an active session.
    /// </summary>
    public void SeenNow()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update a revoked session.");

        LastSeenUtc = DateTime.UtcNow;
        Touch();
    }

    /// <summary>
    /// Revokes the session, raising <see cref="SessionRevoked"/>.
    /// </summary>
    public void Revoke(string reason)
    {
        if (!IsActive)
            throw new InvalidOperationException("Session already revoked.");

        RevokedUtc = DateTime.UtcNow;
        RevokeReason = reason;
        Raise(new SessionRevoked(UserId, Id, reason));
        Touch();
    }
}
