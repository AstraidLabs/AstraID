using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.Options;

public class AstraIdOptions
{
    [Required]
    [Url]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string[] AllowedCors { get; set; } = Array.Empty<string>();

    public RateLimitOptions RateLimit { get; set; } = new();

    public bool AutoMigrate { get; set; }
    public bool RunSeed { get; set; }
}

public class RateLimitOptions
{
    [Range(1, 1000)]
    public int Rps { get; set; }

    [Range(1, 1000)]
    public int Burst { get; set; }
}
