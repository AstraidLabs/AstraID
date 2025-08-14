using System.Text.RegularExpressions;

namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a culture code (e.g., en-US).
/// </summary>
public sealed record CultureCode
{
    private static readonly Regex Pattern = new("^[a-z]{2}(-[A-Z]{2})?$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the culture code value.
    /// </summary>
    public string Value { get; }

    private CultureCode(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="CultureCode"/>.
    /// </summary>
    public static CultureCode Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Culture code cannot be empty.", nameof(input));

        var trimmed = input.Trim();
        if (!Pattern.IsMatch(trimmed))
            throw new ArgumentException("Invalid culture code format.", nameof(input));

        return new CultureCode(trimmed);
    }

    public override string ToString() => Value;
}
