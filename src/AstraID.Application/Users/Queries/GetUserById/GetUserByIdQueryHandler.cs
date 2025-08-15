using AstraID.Application.Common;
using AstraID.Application.Common.Errors;
using AstraID.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace AstraID.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IAppUserRepository _users;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(IAppUserRepository users, IMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user == null)
            return Result<UserDto>.Failure(AppErrorCodes.UserNotFound, "User not found.");

        var dto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(dto);
    }
}
