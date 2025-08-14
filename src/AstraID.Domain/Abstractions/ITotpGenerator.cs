namespace AstraID.Domain.Abstractions;

/// <summary>Generates and validates time-based one-time passwords.</summary>
public interface ITotpGenerator
{
    /// <summary>Validates a TOTP code for a shared secret.</summary>
    bool Validate(string sharedSecret, string code, DateTime utcNow);
}
