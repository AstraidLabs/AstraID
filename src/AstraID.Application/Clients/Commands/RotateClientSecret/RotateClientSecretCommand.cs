using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Clients.Commands.RotateClientSecret;

public sealed record RotateClientSecretCommand(string ClientId, string NewSecret)
    : IRequest<Result>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "clients.write" };
    public string[] RequiredRoles => new[] { "Admin" };
}
