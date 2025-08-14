using Microsoft.AspNetCore.Authorization;

namespace AstraID.Shared.Security;

public class ScopeRequirement(string scope) : IAuthorizationRequirement
{
    public string Scope { get; } = scope;
}
