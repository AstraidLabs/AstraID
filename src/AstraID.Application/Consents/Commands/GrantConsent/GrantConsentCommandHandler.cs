using System.Linq;
using AstraID.Application.Common;
using AstraID.Domain.Services;
using AstraID.Domain.ValueObjects;
using MediatR;

namespace AstraID.Application.Consents.Commands.GrantConsent;

public sealed class GrantConsentCommandHandler : IRequestHandler<GrantConsentCommand, Result>
{
    private readonly ConsentDomainService _consents;

    public GrantConsentCommandHandler(ConsentDomainService consents) => _consents = consents;

    public async Task<Result> Handle(GrantConsentCommand request, CancellationToken ct)
    {
        var scopes = request.Scopes.Select(Scope.Create);
        var result = await _consents.GrantAsync(request.UserId, request.ClientId, scopes, ct);
        if (!result.IsSuccess)
            return Result.Failure(result.Error!.Code, result.Error!.Message);
        return Result.Success();
    }
}
