using AstraID.Domain.Entities;
using AstraID.Persistence;
using AstraID.Shared;
using AstraID.Shared.Options;
using AstraID.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AstraID.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityAndAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AstraIdDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
        })
        .AddCookie(IdentityConstants.ApplicationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            o.Authority = configuration["ASTRAID_ISSUER"];
            o.RequireHttpsMetadata = false;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AstraIdPolicies.ApiRead, p =>
            {
                p.RequireAuthenticatedUser();
                p.Requirements.Add(new ScopeRequirement(AstraIdScopes.ApiRead));
            });
            options.AddPolicy(AstraIdPolicies.ApiWrite, p =>
            {
                p.RequireAuthenticatedUser();
                p.Requirements.Add(new ScopeRequirement(AstraIdScopes.ApiWrite));
            });
            options.AddPolicy(AstraIdPolicies.AdminWrite, p =>
            {
                p.RequireRole("Admin");
                p.Requirements.Add(new ScopeRequirement(AstraIdScopes.ApiWrite));
            });
        });

        services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
        return services;
    }

    public static IServiceCollection AddOpenIddictServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(opt => opt.UseEntityFrameworkCore().UseDbContext<AstraIdDbContext>())
            .AddServer(opt =>
            {
                var issuer = configuration["ASTRAID_ISSUER"] ?? throw new InvalidOperationException("Issuer not configured");
                opt.SetIssuer(issuer);
                opt.AllowAuthorizationCodeFlow()
                    .AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow();
                opt.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token");
                opt.RegisterScopes(AstraIdScopes.ApiRead, AstraIdScopes.ApiWrite, "profile", "email", "roles", "offline_access");
                opt.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough();
            })
            .AddValidation(opt =>
            {
                
                opt.UseAspNetCore();
            });
        return services;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration["ASTRAID_DB_CONN"] ?? string.Empty, name: "db", tags: new[] { "ready" });
        services.AddOpenTelemetry().WithTracing(b => { });
        return services;
    }

    public static IServiceCollection AddAstraIdOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AstraIdOptions>(configuration);
        return services;
    }
}
