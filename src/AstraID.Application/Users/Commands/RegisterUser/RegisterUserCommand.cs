using MediatR;
using AstraID.Application.Behaviors;
using AstraID.Application.Common;

namespace AstraID.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Email, string? DisplayName, string Password)
    : IRequest<Result<Guid>>, IAuthorizedRequest
{
    public string[] RequiredScopes => new[] { "users.write" };
    public string[] RequiredRoles => Array.Empty<string>();
}
