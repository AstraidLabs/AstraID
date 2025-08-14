namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Optional display name value object.
/// </summary>
public sealed record DisplayName
{
    /// <summary>
    /// Gets the display name value. Empty string represents no display name.
    /// </summary>
    public string Value { get; }

    private DisplayName(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="DisplayName"/> instance.
    /// </summary>
    public static DisplayName Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new DisplayName(string.Empty);

        var trimmed = input.Trim();
        if (trimmed.Length < 3 || trimmed.Length > 128)
            throw new ArgumentException("Display name must be between 3 and 128 characters.", nameof(input));

        return new DisplayName(trimmed);
    }

    public override string ToString() => Value;
}
