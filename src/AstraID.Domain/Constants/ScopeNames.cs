using System.Collections.Generic;
namespace AstraID.Domain.Constants;

/// <summary>Well-known scope names.</summary>
public static class ScopeNames
{
    public const string ApiRead = "api.read";
    public const string ApiWrite = "api.write";
    public const string Profile = "profile";
    public const string Email = "email";
    public const string Roles = "roles";
    public const string OfflineAccess = "offline_access";

    /// <summary>Set containing all scope names.</summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        ApiRead, ApiWrite, Profile, Email, Roles, OfflineAccess
    };
}
