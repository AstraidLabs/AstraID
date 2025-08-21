using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using AstraID.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation;
using OpenIddict.Validation.AspNetCore;
using AstraID.Api.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AstraID.Api.OpenIddict;

/// <summary>
/// Configures OpenIddict server and validation components.
/// </summary>
public static class OpenIddictConfig
{
    public static IServiceCollection AddAstraIdOpenIddict(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
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

                // PKCE: only S256
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

                // Certificates
                var certSection = configuration.GetSection("AstraId:Certificates");
                var signingFolder = certSection.GetValue<string>("SigningFolder");
                var signingCerts = LoadFromFolder(signingFolder);
                if (signingCerts.Any())
                {
                    foreach (var c in signingCerts) opt.AddSigningCertificate(c);
                }
                else if (env.IsDevelopment())
                {
                    opt.AddDevelopmentSigningCertificate();
                }
                else
                {
                    throw new InvalidOperationException("No signing certificates found");
                }

                var encryptionFolder = certSection.GetValue<string>("EncryptionFolder");
                var encryptionCerts = LoadFromFolder(encryptionFolder);
                if (encryptionCerts.Any())
                {
                    foreach (var c in encryptionCerts) opt.AddEncryptionCertificate(c);
                }
                else if (env.IsDevelopment())
                {
                    opt.AddDevelopmentEncryptionCertificate();
                }
                else
                {
                    throw new InvalidOperationException("No encryption certificates found");
                }

                // Access token format
                var tokenFormat = configuration["Auth:AccessTokenFormat"] ?? "Jwt";
                if (string.Equals(tokenFormat, "Reference", StringComparison.OrdinalIgnoreCase))
                    opt.UseReferenceAccessTokens();
                else
                    opt.DisableAccessTokenEncryption();

                // PAR enforcement optional
                if (configuration.GetValue<bool>("AstraId:RequireParForPublicClients"))
                    opt.RequirePushedAuthorizationRequests();

                // ASP.NET Core host + passthroughs
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

    private static IEnumerable<X509Certificate2> LoadFromFolder(string? folder)
    {
        var certs = new List<X509Certificate2>();
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            return certs;

        var manifestPath = Path.Combine(folder, "manifest.json");
        if (File.Exists(manifestPath))
        {
            var json = File.ReadAllText(manifestPath);
            var entries = JsonSerializer.Deserialize<List<KeyManifestEntry>>(json) ?? new();
            var ordered = entries.Where(e => File.Exists(e.Path))
                .OrderBy(e => e.Status == KeyStatus.Active ? 1 : 0);
            foreach (var e in ordered)
            {
                certs.Add(new X509Certificate2(e.Path));
            }
        }
        else
        {
            foreach (var file in Directory.GetFiles(folder, "*.pfx").OrderBy(f => f))
            {
                certs.Add(new X509Certificate2(file));
            }
        }
        return certs;
    }
}
