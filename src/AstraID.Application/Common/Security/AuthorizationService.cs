using System.Linq;
using AstraID.Application.Abstractions;

namespace AstraID.Application.Common.Security;

/// <summary>
/// Default authorization service evaluating scopes and roles against the current user.
/// </summary>
public sealed class AuthorizationService : IAuthorizationService
{
    private readonly ICurrentUser _currentUser;

    public AuthorizationService(ICurrentUser currentUser) => _currentUser = currentUser;

    public Task<bool> AuthorizeAsync(IEnumerable<string> requiredScopes, IEnumerable<string> requiredRoles, CancellationToken ct = default)
    {
        var scopeOk = requiredScopes.All(_currentUser.HasScope);
        var roleOk = requiredRoles.All(_currentUser.IsInRole);
        return Task.FromResult(scopeOk && roleOk);
    }
}
