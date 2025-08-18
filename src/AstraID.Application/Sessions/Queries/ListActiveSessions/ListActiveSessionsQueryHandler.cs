using AstraID.Application.Common;
using AstraID.Domain.Repositories;
using MapsterMapper;
using MediatR;
using System.Linq;

namespace AstraID.Application.Sessions.Queries.ListActiveSessions;

public sealed class ListActiveSessionsQueryHandler : IRequestHandler<ListActiveSessionsQuery, Result<IReadOnlyList<SessionDto>>>
{
    private readonly IUserSessionRepository _sessions;
    private readonly IMapper _mapper;

    public ListActiveSessionsQueryHandler(IUserSessionRepository sessions, IMapper mapper)
    {
        _sessions = sessions;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SessionDto>>> Handle(ListActiveSessionsQuery request, CancellationToken ct)
    {
        var list = await _sessions.ListActiveByUserAsync(request.UserId, ct);
        var dto = list.Select(s => _mapper.Map<SessionDto>(s)).ToList();
        return Result<IReadOnlyList<SessionDto>>.Success(dto);
    }
}
