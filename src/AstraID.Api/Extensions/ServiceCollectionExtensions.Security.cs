using System;
using System.Linq;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstraID.Api.Extensions;

/// <summary>
/// Registers security related services such as CORS, authentication/authorization policies and rate limiting.
/// </summary>
public static class ServiceCollectionExtensionsSecurity
{
    public static IServiceCollection AddAstraIdSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = (configuration["ASTRAID_ALLOWED_CORS"] ?? string.Empty)
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(options =>
        {
            options.AddPolicy("cors", p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = configuration["ASTRAID_ISSUER"];
                o.RequireHttpsMetadata = true;
                o.TokenValidationParameters.ValidateAudience = false;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("require.users.write", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("scope", "users.write") || ctx.User.IsInRole("Admin"));
            });
            options.AddPolicy("require.clients.write", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
                policy.RequireClaim("scope", "clients.write");
            });
        });

        var rps = int.TryParse(configuration["ASTRAID_RATE_LIMIT_RPS"], out var r) ? r : 10;
        var burst = int.TryParse(configuration["ASTRAID_RATE_LIMIT_BURST"], out var b) ? b : 20;

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetTokenBucketLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = burst,
                        TokensPerPeriod = rps,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = burst
                    }));
        });

        return services;
    }
}
