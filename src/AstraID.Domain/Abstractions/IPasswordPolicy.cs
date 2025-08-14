namespace AstraID.Domain.Abstractions;

/// <summary>Validates password strength.</summary>
public interface IPasswordPolicy
{
    /// <summary>Validates strength (length, charset, entropy) and returns error message or null.</summary>
    string? ValidateStrength(string password);
}
