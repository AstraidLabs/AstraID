using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Sessions.Commands.RevokeSession;

public sealed record RevokeSessionCommand(Guid UserId, string DeviceId, string Reason)
    : IRequest<Result>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "sessions.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
