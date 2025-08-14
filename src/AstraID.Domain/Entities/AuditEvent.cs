using System.Text.Json;
using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents an audit trail entry.
/// </summary>
[Table("AuditEvents", Schema = "auth")]
[Index(nameof(EventType), nameof(CreatedUtc))]
[Index(nameof(UserId), nameof(CreatedUtc))]
[Index(nameof(ClientId), nameof(CreatedUtc))]
public sealed class AuditEvent : AggregateRoot<Guid>
{
    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional tenant identifier.
    /// </summary>
    public Guid? TenantId { get; private set; }

    /// <summary>
    /// User identifier if applicable.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Client identifier if applicable.
    /// </summary>
    [MaxLength(100)]
    public string? ClientId { get; private set; }

    /// <summary>
    /// Type of audit event.
    /// </summary>
    public AuditEventType EventType { get; private set; }

    /// <summary>
    /// Optional data payload serialized as JSON.
    /// </summary>
    public string? DataJson { get; private set; }

    /// <summary>
    /// Optional failure reason.
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Correlation identifier.
    /// </summary>
    [MaxLength(64)]
    public string? Correlation { get; private set; }

    /// <summary>
    /// IP address associated with the event.
    /// </summary>
    [MaxLength(45)]
    public string? Ip { get; private set; }

    /// <summary>
    /// User agent associated with the event.
    /// </summary>
    [MaxLength(512)]
    public string? Agent { get; private set; }

    /// <summary>
    /// Optional resource identifier.
    /// </summary>
    [MaxLength(128)]
    public string? ResourceId { get; private set; }

    /// <summary>
    /// Severity level (Info/Warning/Error).
    /// </summary>
    [MaxLength(16)]
    public string? Severity { get; private set; }

    private AuditEvent()
    {
    }

    private AuditEvent(Guid? tenantId, Guid? userId, string? clientId, AuditEventType type,
        string? dataJson, string? failure, string? correlation, string? ip,
        string? agent, string? resourceId, string? severity) : base(Guid.NewGuid())
    {
        TenantId = tenantId;
        UserId = userId;
        ClientId = clientId;
        EventType = type;
        DataJson = dataJson;
        FailureReason = failure;
        Correlation = correlation;
        Ip = ip;
        Agent = agent;
        ResourceId = resourceId;
        Severity = severity;
        CreatedUtc = DateTime.UtcNow;
        Raise(new AuditRaised(userId, type.ToString()));
    }

    /// <summary>
    /// Creates a new <see cref="AuditEvent"/> instance.
    /// </summary>
    public static AuditEvent From(Guid? tenantId, Guid? userId, string? clientId, AuditEventType type,
        object? data = null, string? failure = null, string? correlation = null,
        string? ip = null, string? agent = null, string? resourceId = null, string? severity = "Info")
    {
        string? dataJson = data is null ? null : JsonSerializer.Serialize(data);
        correlation ??= Guid.NewGuid().ToString("N");
        return new AuditEvent(tenantId, userId, clientId, type, dataJson, failure, correlation, ip, agent, resourceId, severity);
    }
}

/// <summary>
/// Enumeration of audit event types.
/// </summary>
public enum AuditEventType
{
    UserRegistered,
    UserLoginSucceeded,
    UserLoginFailed,
    UserLocked,
    RoleChanged,
    ClientCreated,
    ClientSecretRotated,
    TokenIssued,
    TokenRevoked,
    ConsentGranted,
    ConsentRevoked
}
