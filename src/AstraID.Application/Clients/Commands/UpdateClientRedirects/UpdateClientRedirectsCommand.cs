using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Clients.Commands.UpdateClientRedirects;

public sealed record UpdateClientRedirectsCommand(
    string ClientId,
    string[] RedirectUris,
    string[]? PostLogoutRedirectUris
) : IRequest<Result>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "clients.write" };
    public string[] RequiredRoles => new[] { "Admin" };
}
