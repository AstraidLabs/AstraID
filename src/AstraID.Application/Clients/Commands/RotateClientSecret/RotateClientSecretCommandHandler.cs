using AstraID.Application.Common;
using AstraID.Domain.Services;
using MediatR;

namespace AstraID.Application.Clients.Commands.RotateClientSecret;

public sealed class RotateClientSecretCommandHandler : IRequestHandler<RotateClientSecretCommand, Result>
{
    private readonly ClientDomainService _clients;

    public RotateClientSecretCommandHandler(ClientDomainService clients) => _clients = clients;

    public async Task<Result> Handle(RotateClientSecretCommand request, CancellationToken ct)
    {
        var result = await _clients.RotateSecretAsync(request.ClientId, request.NewSecret, ct);
        if (!result.IsSuccess)
            return Result.Failure(result.Error!.Code, result.Error!.Message);
        return Result.Success();
    }
}
