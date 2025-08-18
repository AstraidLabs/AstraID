using System;
using AstraID.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AstraID.Api.OpenIddict;

/// <summary>
/// Configures OpenIddict server and validation components.
/// </summary>
public static class OpenIddictConfig
{
    public static IServiceCollection AddAstraIdOpenIddict(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(opt => opt.UseEntityFrameworkCore().UseDbContext<AstraIdDbContext>())
            .AddServer(opt =>
            {
                var issuer = configuration["ASTRAID_ISSUER"] ?? throw new InvalidOperationException("ASTRAID_ISSUER not set");
                opt.SetIssuer(new Uri(issuer));

                opt.SetAuthorizationEndpointUris("/connect/authorize")
                   .SetTokenEndpointUris("/connect/token")
                   .SetIntrospectionEndpointUris("/connect/introspect");

                opt.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
                   .AllowClientCredentialsFlow()
                   .AllowRefreshTokenFlow();

                opt.RegisterScopes("profile", "email", "roles", "offline_access", "api.read", "api.write");

                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
                opt.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

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
}
