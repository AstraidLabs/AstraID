using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AstraID.Api.Options;

namespace AstraID.Api.Services;

public class SigningKeyRotationService : BackgroundService
{
    private readonly CertificateStoreOptions _options;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<SigningKeyRotationService> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private const int RotationIntervalDays = 30;

    public SigningKeyRotationService(IOptions<CertificateStoreOptions> options, IHostApplicationLifetime lifetime, ILogger<SigningKeyRotationService> logger)
    {
        _options = options.Value;
        _lifetime = lifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessFolderAsync(_options.SigningFolder, stoppingToken);
                await ProcessFolderAsync(_options.EncryptionFolder, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rotating certificates");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    internal async Task ProcessFolderAsync(string folder, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return;
        Directory.CreateDirectory(folder);
        var manifestPath = Path.Combine(folder, "manifest.json");
        var manifest = await LoadManifestAsync(manifestPath, ct);
        var now = DateTimeOffset.UtcNow;

        // prune
        var expired = manifest.Where(k => k.Status == KeyStatus.Grace && k.GraceUntil <= now).ToList();
        foreach (var e in expired)
        {
            if (File.Exists(e.Path))
                File.Delete(e.Path);
            manifest.Remove(e);
        }

        var active = manifest.FirstOrDefault(k => k.Status == KeyStatus.Active);
        if (active == null || active.RotateAfter <= now)
        {
            _logger.LogInformation("Generating new certificate in {Folder}", folder);
            var newEntry = await GenerateCertificateAsync(folder, ct);
            foreach (var e in manifest)
            {
                if (e.Status == KeyStatus.Active)
                {
                    e.Status = KeyStatus.Grace;
                    e.GraceUntil = now.AddDays(_options.GraceDays);
                }
            }
            manifest.Add(newEntry);
            await SaveManifestAsync(manifestPath, manifest, ct);
            _lifetime.StopApplication();
            return;
        }

        await SaveManifestAsync(manifestPath, manifest, ct);
    }

    private static async Task<List<KeyManifestEntry>> LoadManifestAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
            return new List<KeyManifestEntry>();
        await using var stream = File.OpenRead(path);
        var manifest = await JsonSerializer.DeserializeAsync<List<KeyManifestEntry>>(stream, JsonOpts, ct);
        return manifest ?? new List<KeyManifestEntry>();
    }

    private static async Task SaveManifestAsync(string path, List<KeyManifestEntry> manifest, CancellationToken ct)
    {
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, manifest, JsonOpts, ct);
    }

    private static async Task<KeyManifestEntry> GenerateCertificateAsync(string folder, CancellationToken ct)
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var req = new CertificateRequest("CN=AstraID", ecdsa, HashAlgorithmName.SHA256);
        var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddYears(1));
        var export = cert.Export(X509ContentType.Pfx);
        var kid = cert.Thumbprint;
        var path = System.IO.Path.Combine(folder, kid + ".pfx");
        await File.WriteAllBytesAsync(path, export, ct);
        return new KeyManifestEntry
        {
            Kid = kid!,
            Path = path,
            CreatedAt = DateTimeOffset.UtcNow,
            RotateAfter = DateTimeOffset.UtcNow.AddDays(RotationIntervalDays),
            Status = KeyStatus.Active
        };
    }
}

public enum KeyStatus
{
    Active,
    Grace
}

public class KeyManifestEntry
{
    [JsonPropertyName("kid")] public string Kid { get; set; } = string.Empty;
    [JsonPropertyName("path")] public string Path { get; set; } = string.Empty;
    [JsonPropertyName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("rotateAfter")] public DateTimeOffset RotateAfter { get; set; }
    [JsonPropertyName("graceUntil")] public DateTimeOffset GraceUntil { get; set; }
    [JsonPropertyName("status")] public KeyStatus Status { get; set; }
}
