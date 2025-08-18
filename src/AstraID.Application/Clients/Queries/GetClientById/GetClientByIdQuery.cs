using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Clients.Queries.GetClientById;

public sealed record GetClientByIdQuery(string ClientId) : IRequest<Result<ClientDto>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "clients.read" };
    public string[] RequiredRoles => Array.Empty<string>();
}

public sealed record ClientDto(
    Guid Id,
    string ClientId,
    string DisplayName,
    string Type,
    string[] Scopes,
    string[] RedirectUris,
    string[] PostLogoutRedirectUris
);
