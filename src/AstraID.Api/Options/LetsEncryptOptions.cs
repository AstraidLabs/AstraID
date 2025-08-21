using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AstraID.Api.Options;

public class LetsEncryptOptions
{
    public bool Enabled { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Url]
    public string? DirectoryUrl { get; set; }

    public string[] Domains { get; set; } = Array.Empty<string>();

    public string CertPath { get; set; } = string.Empty;

    [Range(1, 60)]
    public int RenewWhenDaysLeft { get; set; } = 30;
}

public class LetsEncryptOptionsValidator : IValidateOptions<LetsEncryptOptions>
{
    public ValidateOptionsResult Validate(string? name, LetsEncryptOptions options)
    {
        var errors = new List<string>();
        if (options.Enabled)
        {
            if (string.IsNullOrWhiteSpace(options.Email))
                errors.Add("AstraId:LetsEncrypt:Email is required when enabled");
            if (string.IsNullOrWhiteSpace(options.DirectoryUrl))
                errors.Add("AstraId:LetsEncrypt:DirectoryUrl is required when enabled");
            if (options.Domains == null || options.Domains.Length == 0)
                errors.Add("AstraId:LetsEncrypt:Domains must contain at least one entry when enabled");
            if (string.IsNullOrWhiteSpace(options.CertPath))
                errors.Add("AstraId:LetsEncrypt:CertPath is required when enabled");
        }
        return errors.Count > 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}
