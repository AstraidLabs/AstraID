using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.Options;

public class CertificateStoreOptions
{
    [Required]
    public string SigningFolder { get; set; } = string.Empty;

    [Required]
    public string EncryptionFolder { get; set; } = string.Empty;

    [Range(0, 365)]
    public int GraceDays { get; set; } = 7;
}
