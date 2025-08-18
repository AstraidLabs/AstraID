using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Consents.Queries.ListUserConsents;

public sealed record ListUserConsentsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<ConsentDto>>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "consents.read" };
    public string[] RequiredRoles => Array.Empty<string>();
}

public sealed record ConsentDto(string ClientId, string[] Scopes, DateTime GrantedUtc);
