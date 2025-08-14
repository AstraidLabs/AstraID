namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents an IP address value object (no heavy parsing).
/// </summary>
public sealed record IpAddress
{
    /// <summary>
    /// Gets the address value or empty string if none.
    /// </summary>
    public string Value { get; }

    private IpAddress(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="IpAddress"/> instance.
    /// </summary>
    public static IpAddress Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new IpAddress(string.Empty);

        return new IpAddress(input.Trim());
    }

    public override string ToString() => Value;
}
