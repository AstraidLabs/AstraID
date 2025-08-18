using System;
using AstraID.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AstraID.Api.Infrastructure.Audit;

public interface IAuditLogger
{
    void Log(string action, string targetId);
}

/// <summary>
/// Simple audit logger that writes structured logs for admin actions.
/// </summary>
public sealed class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;
    private readonly ICurrentUser _currentUser;

    public AuditLogger(ILogger<AuditLogger> logger, ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public void Log(string action, string targetId)
    {
        _logger.LogInformation("Audit {Action} by {UserId} on {TargetId} at {UtcNow}", action, _currentUser.UserId, targetId, DateTimeOffset.UtcNow);
    }
}
