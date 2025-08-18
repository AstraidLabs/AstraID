using AstraID.Application.Common;
using AstraID.Application.Common.Errors;
using AstraID.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace AstraID.Application.Clients.Queries.GetClientById;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientDto>>
{
    private readonly IClientRepository _clients;
    private readonly IMapper _mapper;

    public GetClientByIdQueryHandler(IClientRepository clients, IMapper mapper)
    {
        _clients = clients;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken ct)
    {
        var client = await _clients.GetByClientIdAsync(request.ClientId, null, ct);
        if (client == null)
            return Result<ClientDto>.Failure(AppErrorCodes.ClientNotFound, "Client not found.");
        var dto = _mapper.Map<ClientDto>(client);
        return Result<ClientDto>.Success(dto);
    }
}
