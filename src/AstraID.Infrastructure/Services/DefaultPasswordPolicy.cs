using System.Linq;
using AstraID.Domain.Abstractions;

namespace AstraID.Infrastructure.Services;

/// <summary>Simple password policy enforcing basic complexity rules.</summary>
public sealed class DefaultPasswordPolicy : IPasswordPolicy
{
    /// <inheritdoc />
    public string? ValidateStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            return "Password must be at least 12 characters long.";
        if (!password.Any(char.IsUpper))
            return "Password must contain an uppercase letter.";
        if (!password.Any(char.IsLower))
            return "Password must contain a lowercase letter.";
        if (!password.Any(char.IsDigit))
            return "Password must contain a digit.";
        return null;
    }
}
