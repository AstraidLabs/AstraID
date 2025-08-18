using AstraID.Application.Behaviors;
using AstraID.Application.Common;
using MediatR;

namespace AstraID.Application.Consents.Commands.RevokeConsent;

public sealed record RevokeConsentCommand(Guid UserId, string ClientId)
    : IRequest<Result>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "consents.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
