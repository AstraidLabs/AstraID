using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Consents.Commands.GrantConsent;

public sealed record GrantConsentCommand(Guid UserId, string ClientId, string[] Scopes)
    : IRequest<Result>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "consents.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
