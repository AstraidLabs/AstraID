using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

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

    [Range(1, 5000)]
    public int Burst { get; set; }
}

public class AstraIdOptionsValidator : IValidateOptions<AstraIdOptions>
{
    private readonly IHostEnvironment _env;

    public AstraIdOptionsValidator(IHostEnvironment env)
        => _env = env;

    public ValidateOptionsResult Validate(string? name, AstraIdOptions options)
    {
        var errors = new List<string>();
        bool HasPlaceholder(string value) =>
            value.Contains("env:", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("secret:", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("file:", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("${");

        if (!Uri.TryCreate(options.Issuer, UriKind.Absolute, out var issuer) || issuer.Scheme != Uri.UriSchemeHttps || HasPlaceholder(options.Issuer))
        {
            errors.Add(ValidationError.Format("AstraId:Issuer", "must be an absolute HTTPS URL", "Edit appsettings.json and set AstraId:Issuer to e.g. https://id.example.com"));
        }

        for (var i = 0; i < options.AllowedCors.Length; i++)
        {
            var origin = options.AllowedCors[i];
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
                errors.Add(ValidationError.Format($"AstraId:AllowedCors[{i}]", "must be an absolute URL", "Edit appsettings.json and use full URL like https://app.example.com"));
            }
            else if (uri.Scheme != Uri.UriSchemeHttps && !(_env.IsDevelopment() && uri.Scheme == Uri.UriSchemeHttp))
            {
                errors.Add(ValidationError.Format($"AstraId:AllowedCors[{i}]", "must use HTTPS", "Edit appsettings.json and use https:// origin or enable only in Development"));
            }
        }

        if (options.RateLimit.Rps < 1 || options.RateLimit.Rps > 1000)
            errors.Add(ValidationError.Format("AstraId:RateLimit:Rps", "must be between 1 and 1000", "Edit appsettings.json and set AstraId:RateLimit:Rps within 1-1000"));

        if (options.RateLimit.Burst < 1 || options.RateLimit.Burst > 5000)
            errors.Add(ValidationError.Format("AstraId:RateLimit:Burst", "must be between 1 and 5000", "Edit appsettings.json and set AstraId:RateLimit:Burst within 1-5000"));

        if (options.RateLimit.Burst < options.RateLimit.Rps)
            errors.Add(ValidationError.Format("AstraId:RateLimit", "Burst must be greater than or equal to Rps", "Edit appsettings.json and increase Burst or lower Rps"));

        return errors.Count > 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}
