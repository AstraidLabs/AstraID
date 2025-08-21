using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.IO;
using AstraID.Api.Options;

namespace AstraID.Api.Tls;

public class LetsEncryptCertificateLoader : IDisposable
{
    private readonly string _certPath;
    private readonly ILogger<LetsEncryptCertificateLoader> _logger;
    private readonly FileSystemWatcher _watcher;
    private X509Certificate2? _certificate;

    public LetsEncryptCertificateLoader(IOptions<LetsEncryptOptions> options, ILogger<LetsEncryptCertificateLoader> logger)
    {
        _certPath = options.Value.CertPath;
        _logger = logger;
        Directory.CreateDirectory(_certPath);
        LoadCertificate();
        _watcher = new FileSystemWatcher(_certPath)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size,
            Filter = "*.pem",
            EnableRaisingEvents = true
        };
        _watcher.Changed += (_, __) => LoadCertificate();
        _watcher.Created += (_, __) => LoadCertificate();
    }

    public X509Certificate2? Certificate => _certificate;

    private void LoadCertificate()
    {
        try
        {
            var full = Path.Combine(_certPath, "fullchain.pem");
            var key = Path.Combine(_certPath, "privkey.pem");
            if (!File.Exists(full) || !File.Exists(key))
            {
                _logger.LogWarning("Let's Encrypt certificate files not found at {Path}", _certPath);
                return;
            }
            var cert = X509Certificate2.CreateFromPemFile(full, key);
            cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
            Interlocked.Exchange(ref _certificate, cert);
            _logger.LogInformation("Let's Encrypt certificate loaded");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Let's Encrypt certificate");
        }
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _certificate?.Dispose();
    }
}
