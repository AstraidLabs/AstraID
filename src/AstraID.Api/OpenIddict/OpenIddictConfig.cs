using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
                var issuer = configuration["AstraId:Issuer"] ?? configuration["ASTRAID_ISSUER"]
                             ?? throw new InvalidOperationException("Issuer not configured");
                opt.SetIssuer(new Uri(issuer));

                opt.SetAuthorizationEndpointUris("/connect/authorize")
                   .SetTokenEndpointUris("/connect/token")
                   .SetUserInfoEndpointUris("/connect/userinfo")
                   .SetIntrospectionEndpointUris("/connect/introspect")
                   .SetRevocationEndpointUris("/connect/revocation")
                   .SetEndSessionEndpointUris("/connect/logout")
                   .SetJsonWebKeySetEndpointUris("/connect/jwks")
                   .SetPushedAuthorizationEndpointUris("/connect/par");

                opt.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
                   .AllowClientCredentialsFlow()
                   .AllowRefreshTokenFlow();

                // PKCE: jen S256
                opt.Configure(o =>
                {
                    o.CodeChallengeMethods.Clear();
                    o.CodeChallengeMethods.Add(OpenIddictConstants.CodeChallengeMethods.Sha256);
                });

                // Scopes
                var standard = new[] { Scopes.OpenId, Scopes.Profile, Scopes.Email, Scopes.Phone, Scopes.Address, Scopes.OfflineAccess };
                var custom = configuration.GetSection("Auth:Scopes").Get<string[]>() ?? Array.Empty<string>();
                opt.RegisterScopes(standard.Concat(custom).ToArray());

                // Token lifetimes
                var lifetimes = configuration.GetSection("Auth:TokenLifetimes");
                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(lifetimes.GetValue("AccessMinutes", 60)));
                opt.SetIdentityTokenLifetime(TimeSpan.FromMinutes(lifetimes.GetValue("IdentityMinutes", 15)));
                opt.SetRefreshTokenLifetime(TimeSpan.FromDays(lifetimes.GetValue("RefreshDays", 14)));

                // Certy
                var signing = LoadCertificates(configuration.GetSection("Auth:Certificates:Signing"));
                if (signing.Any()) foreach (var c in signing) opt.AddSigningCertificate(c); else opt.AddDevelopmentSigningCertificate();

                var encrypt = LoadCertificates(configuration.GetSection("Auth:Certificates:Encryption"));
                if (encrypt.Any()) foreach (var c in encrypt) opt.AddEncryptionCertificate(c); else opt.AddDevelopmentEncryptionCertificate();

                // Access token format
                var tokenFormat = configuration["Auth:AccessTokenFormat"] ?? "Jwt";
                if (string.Equals(tokenFormat, "Reference", StringComparison.OrdinalIgnoreCase))
                    opt.UseReferenceAccessTokens();
                else
                    opt.DisableAccessTokenEncryption();

                // PAR vynucení (volitelnì)
                if (configuration.GetValue<bool>("AstraId:RequireParForPublicClients"))
                    opt.RequirePushedAuthorizationRequests();

                // ASP.NET Core host + passthroughy
                opt.UseAspNetCore()
                   .EnableAuthorizationEndpointPassthrough()
                   .EnableTokenEndpointPassthrough()
                   .EnableUserInfoEndpointPassthrough();
            })
            .AddValidation(opt =>
            {
                var mode = configuration.GetValue<string>("Auth:ValidationMode");
                if (string.Equals(mode, "Introspection", StringComparison.OrdinalIgnoreCase))
                {
                    var cid = configuration["Auth:Introspection:ClientId"] ?? throw new InvalidOperationException("Missing Auth:Introspection:ClientId");
                    var csec = configuration["Auth:Introspection:ClientSecret"] ?? throw new InvalidOperationException("Missing Auth:Introspection:ClientSecret");
                    opt.UseIntrospection().SetClientId(cid).SetClientSecret(csec);
                }
                else
                {
                    opt.UseLocalServer();
                }
                opt.UseAspNetCore();
            });


        return services;
    }

    private static IEnumerable<X509Certificate2> LoadCertificates(IConfigurationSection section)
    {
        var certs = new List<X509Certificate2>();

        foreach (var child in section.GetChildren())
        {
            var path = child["Path"];
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                continue;

            var password = child["Password"];
            X509Certificate2 cert = string.IsNullOrEmpty(password)
                ? new X509Certificate2(path)
                : new X509Certificate2(path, password, X509KeyStorageFlags.MachineKeySet);

            certs.Add(cert);
        }

        return certs;
    }
}
