using System.Linq;
using AstraID.Application.Common;
using AstraID.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace AstraID.Application.Consents.Queries.ListUserConsents;

public sealed class ListUserConsentsQueryHandler : IRequestHandler<ListUserConsentsQuery, Result<IReadOnlyList<ConsentDto>>>
{
    private readonly IUserConsentRepository _consents;
    private readonly IMapper _mapper;

    public ListUserConsentsQueryHandler(IUserConsentRepository consents, IMapper mapper)
    {
        _consents = consents;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ConsentDto>>> Handle(ListUserConsentsQuery request, CancellationToken ct)
    {
        var list = await _consents.ListByUserAsync(request.UserId, ct);
        var dto = list.Select(c => _mapper.Map<ConsentDto>(c)).ToList();
        return Result<IReadOnlyList<ConsentDto>>.Success(dto);
    }
}
