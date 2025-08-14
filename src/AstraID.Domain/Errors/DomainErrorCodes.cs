namespace AstraID.Domain.Errors;

/// <summary>Common domain error codes.</summary>
public static class DomainErrorCodes
{
    /// <summary>Email already exists.</summary>
    public const string EmailAlreadyExists = "user.email.exists";

    /// <summary>Weak password provided.</summary>
    public const string WeakPassword = "password.weak";

    /// <summary>Password reuse detected.</summary>
    public const string PasswordReuse = "password.reuse";

    /// <summary>Client already exists.</summary>
    public const string ClientExists = "client.exists";

    /// <summary>Invalid scope requested.</summary>
    public const string InvalidScope = "scope.invalid";

    /// <summary>Invalid redirect URI.</summary>
    public const string InvalidRedirectUri = "client.redirect.invalid";

    /// <summary>Secret rotation attempted too soon.</summary>
    public const string SecretRotationTooSoon = "client.secret.rotation.tooSoon";
}
