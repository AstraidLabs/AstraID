using System.Text.Json;
using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents an audit trail entry.
/// </summary>
public sealed class AuditEvent : AggregateRoot<Guid>
{
    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

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
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Correlation identifier.
    /// </summary>
    public CorrelationId Correlation { get; private set; } = null!;

    /// <summary>
    /// IP address associated with the event.
    /// </summary>
    public IpAddress? Ip { get; private set; }

    /// <summary>
    /// User agent associated with the event.
    /// </summary>
    public UserAgent? Agent { get; private set; }

    /// <summary>
    /// Optional resource identifier.
    /// </summary>
    public string? ResourceId { get; private set; }

    /// <summary>
    /// Severity level (Info/Warning/Error).
    /// </summary>
    public string? Severity { get; private set; }

    private AuditEvent()
    {
    }

    private AuditEvent(Guid? tenantId, Guid? userId, string? clientId, AuditEventType type,
        string? dataJson, string? failure, CorrelationId correlation, IpAddress? ip,
        UserAgent? agent, string? resourceId, string? severity) : base(Guid.NewGuid())
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
        object? data = null, string? failure = null, CorrelationId? corr = null,
        IpAddress? ip = null, UserAgent? agent = null, string? resourceId = null, string? severity = "Info")
    {
        string? dataJson = data is null ? null : JsonSerializer.Serialize(data);
        var correlation = corr ?? CorrelationId.Create(Guid.NewGuid());
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
