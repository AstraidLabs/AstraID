using AstraID.Application.Common;
using AstraID.Domain.Services;
using MediatR;

namespace AstraID.Application.Sessions.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand, Result>
{
    private readonly SessionDomainService _sessions;

    public RevokeSessionCommandHandler(SessionDomainService sessions) => _sessions = sessions;

    public async Task<Result> Handle(RevokeSessionCommand request, CancellationToken ct)
    {
        var result = await _sessions.RevokeAsync(request.UserId, request.DeviceId, request.Reason, ct);
        if (!result.IsSuccess)
            return Result.Failure(result.Error!.Code, result.Error!.Message);
        return Result.Success();
    }
}
