using AstraID.Application.Common;
using AstraID.Domain.Services;
using MediatR;

namespace AstraID.Application.Sessions.Commands.RevokeAllSessions;

public sealed class RevokeAllSessionsCommandHandler : IRequestHandler<RevokeAllSessionsCommand, Result<int>>
{
    private readonly SessionDomainService _sessions;

    public RevokeAllSessionsCommandHandler(SessionDomainService sessions) => _sessions = sessions;

    public async Task<Result<int>> Handle(RevokeAllSessionsCommand request, CancellationToken ct)
    {
        var count = await _sessions.RevokeAllAsync(request.UserId, request.Reason, ct);
        return Result<int>.Success(count);
    }
}
