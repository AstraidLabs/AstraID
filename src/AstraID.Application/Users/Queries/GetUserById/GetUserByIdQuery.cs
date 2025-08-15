using MediatR;
using AstraID.Application.Behaviors;
using AstraID.Application.Common;

namespace AstraID.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "users.read" };
    public string[] RequiredRoles => Array.Empty<string>();
}

public sealed record UserDto(Guid Id, string Email, string? DisplayName, bool EmailConfirmed, bool IsActive);
