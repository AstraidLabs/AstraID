using System.Linq;
using AstraID.Domain.Constants;
using AstraID.Domain.Entities;
using AstraID.Domain.Errors;
using AstraID.Domain.Results;

namespace AstraID.Domain.Policies;

/// <summary>Validates client secret rules.</summary>
public sealed class ClientSecretPolicy
{
    /// <summary>Validates complexity of the plaintext secret.</summary>
    public Result ValidateSecretPlaintext(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 16)
            return Result.Failure(DomainError.From(DomainErrorCodes.WeakPassword, "Secret is too weak."));
        return Result.Success();
    }

    /// <summary>Validates that the client secret rotation complies with minimum interval.</summary>
    public Result ValidateRotation(Client client, DateTime utcNow)
    {
        var last = client.SecretHistory
            .OrderByDescending(h => h.CreatedUtc)
            .FirstOrDefault();
        if (last != null && utcNow - last.CreatedUtc < SecurityConstants.MinSecretRotationInterval)
            return Result.Failure(DomainError.From(DomainErrorCodes.SecretRotationTooSoon, "Secret rotated too recently."));
        return Result.Success();
    }
}
