using MediatR;
using AstraID.Application.Common;
using AstraID.Domain.ValueObjects;
using AstraID.Domain.Services;

namespace AstraID.Application.Users.Commands.RegisterUser;

/// <summary>
/// Handles user registration requests.
/// </summary>
/// <remarks>
/// Delegates to <see cref="UserDomainService"/> so domain invariants and events
/// remain centralized. This command represents an administrative use case rather
/// than public self-service registration.
/// </remarks>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly UserDomainService _users;

    /// <summary>
    /// Initializes a new instance of the handler.
    /// </summary>
    /// <param name="users">Domain service coordinating registration.</param>
    public RegisterUserCommandHandler(UserDomainService users) => _users = users;

    /// <summary>
    /// Creates a new user if validation succeeds.
    /// </summary>
    /// <param name="request">Command parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The identifier of the created user wrapped in a result.</returns>
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email); // Reuse domain validation.
        var displayName = DisplayName.Create(request.DisplayName);
        var result = await _users.RegisterAsync(email, displayName, request.Password, tenantId: null, ct);
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!.Code, result.Error!.Message); // Map domain error for caller.

        return Result<Guid>.Success(result.Value!.Id);
    }
}

