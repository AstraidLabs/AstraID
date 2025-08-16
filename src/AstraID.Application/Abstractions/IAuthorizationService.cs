namespace AstraID.Application.Abstractions;

public interface IAuthorizationService
{
    Task<bool> AuthorizeAsync(IEnumerable<string> requiredScopes, IEnumerable<string> requiredRoles, CancellationToken ct = default);
}
