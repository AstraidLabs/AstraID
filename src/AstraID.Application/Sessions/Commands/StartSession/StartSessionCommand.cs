using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Sessions.Commands.StartSession;

public sealed record StartSessionCommand(
    Guid UserId,
    string DeviceId,
    string IpAddress,
    string UserAgent
) : IRequest<Result<Guid>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "sessions.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
