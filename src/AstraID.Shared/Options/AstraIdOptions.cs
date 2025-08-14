namespace AstraID.Shared.Options;

public class AstraIdOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string[] AllowedCorsOrigins { get; set; } = Array.Empty<string>();
}
