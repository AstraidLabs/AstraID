using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using AstraID.Api.Options;
using AstraID.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opt = Microsoft.Extensions.Options.Options;

namespace AstraID.Api.Tests;

public class SigningKeyRotationServiceTests
{
    [Fact]
    public async Task RotateAndPruneManifest()
    {
        var dir = Directory.CreateTempSubdirectory();
        try
        {
            var options = Opt.Create(new CertificateStoreOptions
            {
                SigningFolder = dir.FullName,
                EncryptionFolder = dir.FullName,
                GraceDays = 1
            });
            var lifetime = new TestLifetime();
            var svc = new SigningKeyRotationService(options, lifetime, NullLogger<SigningKeyRotationService>.Instance);

            // initial cert
            var cert = CreateCert();
            var kid = cert.Thumbprint!;
            var path = Path.Combine(dir.FullName, kid + ".pfx");
            await File.WriteAllBytesAsync(path, cert.Export(X509ContentType.Pfx));
            var manifest = new List<KeyManifestEntry>
            {
                new KeyManifestEntry
                {
                    Kid = kid,
                    Path = path,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-40),
                    RotateAfter = DateTimeOffset.UtcNow.AddDays(-1),
                    Status = KeyStatus.Active
                }
            };
            await File.WriteAllTextAsync(Path.Combine(dir.FullName, "manifest.json"), JsonSerializer.Serialize(manifest));

            await svc.ProcessFolderAsync(dir.FullName, CancellationToken.None);

            var after = JsonSerializer.Deserialize<List<KeyManifestEntry>>(File.ReadAllText(Path.Combine(dir.FullName, "manifest.json")))!;
            Assert.Equal(2, after.Count);
            Assert.Contains(after, e => e.Status == KeyStatus.Active);
            Assert.Contains(after, e => e.Status == KeyStatus.Grace);
            Assert.True(lifetime.Stopped);

            // expire grace
            var grace = after.Single(e => e.Status == KeyStatus.Grace);
            grace.GraceUntil = DateTimeOffset.UtcNow.AddDays(-1);
            await File.WriteAllTextAsync(Path.Combine(dir.FullName, "manifest.json"), JsonSerializer.Serialize(after));

            await svc.ProcessFolderAsync(dir.FullName, CancellationToken.None);

            var final = JsonSerializer.Deserialize<List<KeyManifestEntry>>(File.ReadAllText(Path.Combine(dir.FullName, "manifest.json")))!;
            Assert.Single(final);
            Assert.DoesNotContain(final, e => e.Kid == kid);
            Assert.False(File.Exists(path));
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
        var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        return new X509Certificate2(cert.Export(X509ContentType.Pfx));
    }

    private class TestLifetime : IHostApplicationLifetime
    {
        public bool Stopped { get; private set; }
        public CancellationToken ApplicationStarted => CancellationToken.None;
        public CancellationToken ApplicationStopping => CancellationToken.None;
        public CancellationToken ApplicationStopped => CancellationToken.None;
        public void StopApplication() => Stopped = true;
    }
}
