namespace AstraID.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Scopes { get; }
    bool HasScope(string scope);
    bool IsInRole(string role);
}
