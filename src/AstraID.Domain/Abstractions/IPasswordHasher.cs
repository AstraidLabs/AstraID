namespace AstraID.Domain.Abstractions;

/// <summary>Provides password hashing services.</summary>
public interface IPasswordHasher
{
    /// <summary>Returns a hash for the given plaintext password.</summary>
    string Hash(string password);

    /// <summary>Verifies plaintext against an existing hash.</summary>
    bool Verify(string hash, string password);
}
