using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using System.IO;
using System.Collections.Generic;

namespace AstraID.Api.Options;

public class AuthOptions
{
    public ValidationMode ValidationMode { get; set; } = ValidationMode.Jwt;
    public TokenLifetimeOptions TokenLifetimes { get; set; } = new();
    public CertificateCollectionOptions Certificates { get; set; } = new();
    public IntrospectionOptions Introspection { get; set; } = new();
}

public enum ValidationMode
{
    Jwt,
    Introspection
}

public class TokenLifetimeOptions
{
    [Range(1, 240)]
    public int AccessMinutes { get; set; } = 60;

    [Range(1, 240)]
    public int IdentityMinutes { get; set; } = 15;

    [Range(1, 90)]
    public int RefreshDays { get; set; } = 14;
}

public class CertificateCollectionOptions
{
    public bool UseDevelopmentCertificates { get; set; }
    public CertificateOptions[] Signing { get; set; } = Array.Empty<CertificateOptions>();
    public CertificateOptions[] Encryption { get; set; } = Array.Empty<CertificateOptions>();
}

public class CertificateOptions
{
    [Required]
    public string Path { get; set; } = string.Empty;
    public string? Password { get; set; }
}

public class IntrospectionOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}

public class AuthOptionsValidator : IValidateOptions<AuthOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthOptions options)
    {
        var errors = new List<string>();
        bool HasPlaceholder(string? value) =>
            value?.Contains("env:", StringComparison.OrdinalIgnoreCase) == true ||
            value?.Contains("secret:", StringComparison.OrdinalIgnoreCase) == true ||
            value?.Contains("file:", StringComparison.OrdinalIgnoreCase) == true ||
            value?.Contains("${") == true;

        if (!options.Certificates.UseDevelopmentCertificates)
        {
            if (options.Certificates.Signing.Length == 0)
                errors.Add(ValidationError.Format("Auth:Certificates:Signing", "at least one signing certificate is required or enable UseDevelopmentCertificates", "Edit appsettings.json and add certificate path or set UseDevelopmentCertificates=true"));
            for (var i = 0; i < options.Certificates.Signing.Length; i++)
            {
                var cert = options.Certificates.Signing[i];
                if (string.IsNullOrWhiteSpace(cert.Path) || HasPlaceholder(cert.Path) || !File.Exists(cert.Path))
                    errors.Add(ValidationError.Format($"Auth:Certificates:Signing[{i}].Path", "file not found", "Edit appsettings.json and ensure the certificate file exists or set UseDevelopmentCertificates=true"));
            }
            for (var i = 0; i < options.Certificates.Encryption.Length; i++)
            {
                var cert = options.Certificates.Encryption[i];
                if (string.IsNullOrWhiteSpace(cert.Path) || HasPlaceholder(cert.Path) || !File.Exists(cert.Path))
                    errors.Add(ValidationError.Format($"Auth:Certificates:Encryption[{i}].Path", "file not found", "Edit appsettings.json and ensure the certificate file exists or set UseDevelopmentCertificates=true"));
            }
        }

        if (options.ValidationMode == ValidationMode.Introspection)
        {
            if (string.IsNullOrWhiteSpace(options.Introspection.ClientId) || HasPlaceholder(options.Introspection.ClientId))
                errors.Add(ValidationError.Format("Auth:Introspection:ClientId", "is required when ValidationMode is 'Introspection'", "Edit appsettings.json and set Auth:Introspection:ClientId"));
            if (string.IsNullOrWhiteSpace(options.Introspection.ClientSecret) || HasPlaceholder(options.Introspection.ClientSecret))
                errors.Add(ValidationError.Format("Auth:Introspection:ClientSecret", "is required when ValidationMode is 'Introspection'", "Edit appsettings.json and set Auth:Introspection:ClientSecret"));
        }

        return errors.Count > 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}
