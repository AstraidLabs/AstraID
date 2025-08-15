using Microsoft.Extensions.Configuration;

namespace AstraID.Infrastructure.Startup;

/// <summary>
/// Strongly-typed options for database seeding.
/// Values are provided via environment variables or user secrets.
/// </summary>
public sealed class SeedOptions
{
    public IReadOnlyList<string> AdminRoles { get; init; } = Array.Empty<string>();
    public string? AdminEmail { get; init; }
    public string? AdminPassword { get; init; }
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
    public string? AdminClientId { get; init; }
    public string? AdminClientSecret { get; init; }
    public string? WebClientId { get; init; }
    public IReadOnlyList<string> WebRedirects { get; init; } = Array.Empty<string>();
    public string? Issuer { get; init; }

    public static SeedOptions FromConfiguration(IConfiguration configuration) => new()
    {
        AdminRoles = ParseList(configuration["ASTRAID_ADMIN_ROLES"]),
        AdminEmail = configuration["ASTRAID_ADMIN_EMAIL"],
        AdminPassword = configuration["ASTRAID_ADMIN_PASSWORD"],
        Scopes = ParseList(configuration["ASTRAID_SCOPES"]),
        AdminClientId = configuration["ASTRAID_ADMIN_CLIENT_ID"],
        AdminClientSecret = configuration["ASTRAID_ADMIN_CLIENT_SECRET"],
        WebClientId = configuration["ASTRAID_WEB_CLIENT_ID"],
        WebRedirects = ParseList(configuration["ASTRAID_WEB_REDIRECTS"]),
        Issuer = configuration["ASTRAID_ISSUER"]
    };

    private static IReadOnlyList<string> ParseList(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<string>();

        return input
            .Split(new[] { ';', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}

