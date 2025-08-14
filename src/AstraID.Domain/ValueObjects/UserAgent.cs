namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a user agent string.
/// </summary>
public sealed record UserAgent
{
    /// <summary>
    /// Gets the agent value or empty string if none.
    /// </summary>
    public string Value { get; }

    private UserAgent(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="UserAgent"/> instance.
    /// </summary>
    public static UserAgent Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new UserAgent(string.Empty);

        return new UserAgent(input.Trim());
    }

    public override string ToString() => Value;
}
