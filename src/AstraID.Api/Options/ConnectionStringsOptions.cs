using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.Common;

namespace AstraID.Api.Options;

public class ConnectionStringsOptions
{
    [Required]
    public string Default { get; set; } = string.Empty;
}

public class ConnectionStringsOptionsValidator : IValidateOptions<ConnectionStringsOptions>
{
    public ValidateOptionsResult Validate(string? name, ConnectionStringsOptions options)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(options.Default))
        {
            errors.Add(ValidationError.Format("ConnectionStrings:Default", "must not be empty", "Provide a valid connection string"));
        }
        else
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = options.Default };
            var hasIntegrated = builder.TryGetValue("Integrated Security", out var integrated) && integrated?.ToString()?.Equals("True", StringComparison.OrdinalIgnoreCase) == true;
            bool hasUser = builder.ContainsKey("User Id") || builder.ContainsKey("UserID") || builder.ContainsKey("UID");
            bool hasPassword = builder.ContainsKey("Password") || builder.ContainsKey("PWD");
            if (!(hasIntegrated || (hasUser && hasPassword)))
            {
                errors.Add(ValidationError.Format("ConnectionStrings:Default", "must include 'Integrated Security=True' or both 'User Id' and 'Password'", "Update connection string credentials"));
            }
        }
        return errors.Count > 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}
