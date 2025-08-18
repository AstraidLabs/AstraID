using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AstraID.Api.Health;

/// <summary>
/// Registers and maps health checks for the API.
/// </summary>
public static class HealthChecksConfig
{
    public static IServiceCollection AddAstraIdHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
            .AddSqlServer(configuration["ASTRAID_DB_CONN"] ?? string.Empty, name: "db", tags: new[] { "ready" });
        return services;
    }

    public static void MapAstraIdHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") })
            .WithTags("Health");
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = r => r.Tags.Contains("ready") })
            .WithTags("Health");
    }
}
