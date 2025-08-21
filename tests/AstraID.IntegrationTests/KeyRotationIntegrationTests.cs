using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Json;
using System.Text.Json;
using AstraID.Api;
using AstraID.Api.Services;
using AstraID.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using System.IdentityModel.Tokens.Jwt;

namespace AstraID.IntegrationTests;

public class RotationFactory : WebApplicationFactory<Program>
{
    private readonly string _folder;
    public RotationFactory(string folder) => _folder = folder;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("AstraId:Certificates:SigningFolder", _folder);
        builder.UseSetting("AstraId:Certificates:EncryptionFolder", _folder);
        builder.UseSetting("Auth:Certificates:UseDevelopmentCertificates", "true");
        builder.UseSetting("ASTRAID_DB_PROVIDER", "Sqlite");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AstraIdDbContext>));
            services.AddDbContext<AstraIdDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        });
    }
}

public class KeyRotationIntegrationTests
{
    private static async Task SeedClientAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "client",
            ClientSecret = "secret",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
            }
        });
    }

    [Fact]
    public async Task JwksContainsGraceAndActive_NewKidSignsTokens()
    {
        var dir = Directory.CreateTempSubdirectory();
        try
        {
            // create old and new certs
            var oldCert = CreateCert();
            var oldKid = oldCert.Thumbprint!;
            var oldPath = Path.Combine(dir.FullName, oldKid + ".pfx");
            await File.WriteAllBytesAsync(oldPath, oldCert.Export(X509ContentType.Pfx));

            var newCert = CreateCert();
            var newKid = newCert.Thumbprint!;
            var newPath = Path.Combine(dir.FullName, newKid + ".pfx");
            await File.WriteAllBytesAsync(newPath, newCert.Export(X509ContentType.Pfx));

            var manifest = new List<KeyManifestEntry>
            {
                new KeyManifestEntry { Kid = oldKid, Path = oldPath, CreatedAt = DateTimeOffset.UtcNow.AddDays(-40), RotateAfter = DateTimeOffset.UtcNow.AddDays(-10), GraceUntil = DateTimeOffset.UtcNow.AddDays(10), Status = KeyStatus.Grace },
                new KeyManifestEntry { Kid = newKid, Path = newPath, CreatedAt = DateTimeOffset.UtcNow, RotateAfter = DateTimeOffset.UtcNow.AddDays(30), Status = KeyStatus.Active }
            };
            await File.WriteAllTextAsync(Path.Combine(dir.FullName, "manifest.json"), JsonSerializer.Serialize(manifest));

            await using var factory = new RotationFactory(dir.FullName);
            await SeedClientAsync(factory.Services);
            var client = factory.CreateClient();

            var jwks = await client.GetFromJsonAsync<JsonElement>("/connect/jwks");
            var kids = jwks.GetProperty("keys").EnumerateArray().Select(k => k.GetProperty("kid").GetString()).ToList();
            Assert.Contains(oldKid, kids);
            Assert.Contains(newKid, kids);

            var resp = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = "client",
                ["client_secret"] = "secret"
            }));
            var tokenJson = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var token = tokenJson.GetProperty("access_token").GetString();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Assert.Equal(newKid, jwt.Header.Kid);
        }
        finally
        {
            dir.Delete(true);
        }
    }

    private static X509Certificate2 CreateCert()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var req = new CertificateRequest("CN=Test", ecdsa, HashAlgorithmName.SHA256);
        var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
        return new X509Certificate2(cert.Export(X509ContentType.Pfx));
    }
}
