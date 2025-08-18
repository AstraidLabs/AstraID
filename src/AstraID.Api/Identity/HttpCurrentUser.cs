using System;
using System.Linq;
using System.Security.Claims;
using AstraID.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AstraID.Api.Identity;

/// <summary>
/// Adapter reading the current authenticated user from the HTTP context.
/// </summary>
public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public HttpCurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal User => _accessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid? UserId => Guid.TryParse(User.FindFirstValue("sub"), out var id) ? id : null;

    public string? Email => User.FindFirstValue(ClaimTypes.Email);

    public IReadOnlyCollection<string> Roles => User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

    public IReadOnlyCollection<string> Scopes =>
        User.FindAll("scope").SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();

    public bool HasScope(string scope) => Scopes.Contains(scope);

    public bool IsInRole(string role) => User.IsInRole(role);
}
