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
                var issuer = configuration["ASTRAID_ISSUER"] ?? configuration["AstraId:Issuer"]
                    ?? throw new InvalidOperationException("Issuer not configured");
                opt.SetIssuer(new Uri(issuer));

                opt.SetAuthorizationEndpointUris("/connect/authorize")
                   .SetTokenEndpointUris("/connect/token")
                   .SetIntrospectionEndpointUris("/connect/introspect")
                   .SetUserInfoEndpointUris("/connect/userinfo")
                   .SetRevocationEndpointUris("/connect/revocation")
                   .SetEndSessionEndpointUris("/connect/logout")
                   .SetJsonWebKeySetEndpointUris("/connect/jwks")
                   .SetPushedAuthorizationEndpointUris("/connect/par");

                opt.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange()
                   .AllowClientCredentialsFlow()
                   .AllowRefreshTokenFlow();

                // Only allow the S256 code challenge method for PKCE
                opt.Configure(options =>
                {
                    options.CodeChallengeMethods.Clear();
                    options.CodeChallengeMethods.Add(CodeChallengeMethods.Sha256);
                });

                var standardScopes = new[]
                {
                    Scopes.OpenId, Scopes.Profile, Scopes.Email,
                    Scopes.Phone, Scopes.Address, Scopes.OfflineAccess
                };
                var customScopes = configuration.GetSection("Auth:Scopes").Get<string[]>() ?? Array.Empty<string>();
                opt.RegisterScopes(standardScopes.Concat(customScopes).ToArray());

                var lifetimes = configuration.GetSection("Auth:TokenLifetimes");
                var access = lifetimes.GetValue<int?>("AccessMinutes") ?? 60;
                var identity = lifetimes.GetValue<int?>("IdentityMinutes") ?? 15;
                var refresh = lifetimes.GetValue<int?>("RefreshDays") ?? 14;
                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(access));
                opt.SetIdentityTokenLifetime(TimeSpan.FromMinutes(identity));
                opt.SetRefreshTokenLifetime(TimeSpan.FromDays(refresh));

                var signingCerts = LoadCertificates(configuration.GetSection("Auth:Certificates:Signing"));
                if (signingCerts.Any())
                {
                    foreach (var cert in signingCerts)
                        opt.AddSigningCertificate(cert);
                }
                else
                {
                    opt.AddDevelopmentSigningCertificate();
                }

                var encryptionCerts = LoadCertificates(configuration.GetSection("Auth:Certificates:Encryption"));
                if (encryptionCerts.Any())
                {
                    foreach (var cert in encryptionCerts)
                        opt.AddEncryptionCertificate(cert);
                }
                else
                {
                    opt.AddDevelopmentEncryptionCertificate();
                }

                var tokenFormat = configuration["Auth:AccessTokenFormat"] ?? "Jwt";
                if (string.Equals(tokenFormat, "Reference", StringComparison.OrdinalIgnoreCase))
                {
                    opt.UseReferenceAccessTokens();
                }
                else
                {
                    opt.DisableAccessTokenEncryption();
                }

                if (configuration.GetValue<bool?>("AstraId:RequireParForPublicClients") == true)
                {
                    opt.RequirePushedAuthorizationRequests();
                }

                opt.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough();
            })
            .AddValidation(opt =>
            {

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
