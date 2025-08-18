using AstraID.Application.Common;
using AstraID.Domain.Services;
using AstraID.Domain.ValueObjects;
using MediatR;

namespace AstraID.Application.Sessions.Commands.StartSession;

public sealed class StartSessionCommandHandler : IRequestHandler<StartSessionCommand, Result<Guid>>
{
    private readonly SessionDomainService _sessions;

    public StartSessionCommandHandler(SessionDomainService sessions) => _sessions = sessions;

    public async Task<Result<Guid>> Handle(StartSessionCommand request, CancellationToken ct)
    {
        var device = DeviceId.Create(request.DeviceId);
        var ip = IpAddress.Create(request.IpAddress);
        var agent = UserAgent.Create(request.UserAgent);
        var result = await _sessions.StartAsync(request.UserId, device, ip, agent, ct);
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!.Code, result.Error!.Message);
        return Result<Guid>.Success(result.Value!.Id);
    }
}
