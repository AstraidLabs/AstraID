using AstraID.Application.Common;
using AstraID.Domain.Services;
using MediatR;

namespace AstraID.Application.Consents.Commands.RevokeConsent;

public sealed class RevokeConsentCommandHandler : IRequestHandler<RevokeConsentCommand, Result>
{
    private readonly ConsentDomainService _consents;

    public RevokeConsentCommandHandler(ConsentDomainService consents) => _consents = consents;

    public async Task<Result> Handle(RevokeConsentCommand request, CancellationToken ct)
    {
        var result = await _consents.RevokeAsync(request.UserId, request.ClientId, ct);
        if (!result.IsSuccess)
            return Result.Failure(result.Error!.Code, result.Error!.Message);
        return Result.Success();
    }
}
