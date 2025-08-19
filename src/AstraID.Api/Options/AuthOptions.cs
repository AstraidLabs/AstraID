using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.Options;

public class AuthOptions
{
    public TokenLifetimeOptions TokenLifetimes { get; set; } = new();

    public CertificateOptions Certificates { get; set; } = new();

    public IntrospectionOptions Introspection { get; set; } = new();
}

public class TokenLifetimeOptions
{
    [Range(1, int.MaxValue)]
    public int AccessMinutes { get; set; } = 60;

    [Range(1, int.MaxValue)]
    public int IdentityMinutes { get; set; } = 15;

    [Range(1, int.MaxValue)]
    public int RefreshDays { get; set; } = 14;
}

public class CertificateOptions
{
    public string[] Signing { get; set; } = Array.Empty<string>();
    public string[] Encryption { get; set; } = Array.Empty<string>();
}

public class IntrospectionOptions
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;
}
