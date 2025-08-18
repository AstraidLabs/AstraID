using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Sessions.Commands.RevokeAllSessions;

public sealed record RevokeAllSessionsCommand(Guid UserId, string Reason)
    : IRequest<Result<int>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "sessions.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
