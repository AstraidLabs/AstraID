using Microsoft.AspNetCore.Authorization;

namespace AstraID.Api.Security;

/// <summary>
/// Authorization policies used by AstraID API.
/// </summary>
public static class Policies
{
    public const string Admin = "admin";

    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(Admin, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("admin");
            policy.RequireClaim("scope", "astra.admin");
        });
    }
}
