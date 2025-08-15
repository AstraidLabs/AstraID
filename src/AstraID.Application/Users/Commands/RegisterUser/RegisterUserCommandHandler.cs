using MediatR;
using AstraID.Application.Common;
using AstraID.Domain.ValueObjects;
using AstraID.Domain.Services;

namespace AstraID.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly UserDomainService _users;
    public RegisterUserCommandHandler(UserDomainService users) => _users = users;

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email);
        var displayName = DisplayName.Create(request.DisplayName);
        var result = await _users.RegisterAsync(email, displayName, request.Password, tenantId: null, ct);
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!.Code, result.Error!.Message);

        return Result<Guid>.Success(result.Value!.Id);
    }
}
