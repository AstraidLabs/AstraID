using System.Text.RegularExpressions;

namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents an OAuth scope.
/// </summary>
public sealed record Scope
{
    private static readonly Regex Pattern = new("^[A-Za-z0-9\\.:-_]+$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the normalized scope value.
    /// </summary>
    public string Value { get; }

    private Scope(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="Scope"/> after validation.
    /// </summary>
    public static Scope Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Scope cannot be empty.", nameof(input));

        var trimmed = input.Trim();
        if (!Pattern.IsMatch(trimmed))
            throw new ArgumentException("Scope contains invalid characters.", nameof(input));

        return new Scope(trimmed.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
