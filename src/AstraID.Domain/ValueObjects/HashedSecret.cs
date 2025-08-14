namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a hashed secret value.
/// </summary>
public sealed record HashedSecret
{
    /// <summary>
    /// Gets the hash value.
    /// </summary>
    public string Value { get; }

    private HashedSecret(string value) => Value = value;

    /// <summary>
    /// Creates a <see cref="HashedSecret"/> from an existing hash string.
    /// </summary>
    public static HashedSecret FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty.", nameof(hash));

        return new HashedSecret(hash);
    }

    public override string ToString() => Value;
}
