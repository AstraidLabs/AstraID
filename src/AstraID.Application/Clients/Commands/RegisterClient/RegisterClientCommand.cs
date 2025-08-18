using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;
using AstraID.Domain.Entities;

namespace AstraID.Application.Clients.Commands.RegisterClient;

public sealed record RegisterClientCommand(
    string ClientId,
    string? DisplayName,
    ClientType Type,
    string[] Scopes,
    string[] RedirectUris,
    string[]? PostLogoutRedirectUris
) : IRequest<Result<Guid>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "clients.write" };
    public string[] RequiredRoles => new[] { "Admin" };
}
