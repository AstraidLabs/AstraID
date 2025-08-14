namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a correlation identifier.
/// </summary>
public sealed record CorrelationId
{
    /// <summary>
    /// Gets the identifier value.
    /// </summary>
    public string Value { get; }

    private CorrelationId(string value) => Value = value;

    /// <summary>
    /// Creates from a string.
    /// </summary>
    public static CorrelationId Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Correlation id cannot be empty.", nameof(input));

        return new CorrelationId(input.Trim());
    }

    /// <summary>
    /// Creates from a <see cref="Guid"/>.
    /// </summary>
    public static CorrelationId Create(Guid guid) => new(guid.ToString());

    public override string ToString() => Value;
}
