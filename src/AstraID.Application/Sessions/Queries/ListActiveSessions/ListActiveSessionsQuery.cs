using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Sessions.Queries.ListActiveSessions;

public sealed record ListActiveSessionsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<SessionDto>>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "sessions.read" };
    public string[] RequiredRoles => Array.Empty<string>();
}

public sealed record SessionDto(
    Guid Id,
    string DeviceId,
    string IpAddress,
    string UserAgent,
    DateTime CreatedUtc,
    DateTime LastSeenUtc
);
