namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a device identifier.
/// </summary>
public sealed record DeviceId
{
    /// <summary>
    /// Gets the device identifier value.
    /// </summary>
    public string Value { get; }

    private DeviceId(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="DeviceId"/>.
    /// </summary>
    public static DeviceId Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Device id cannot be empty.", nameof(input));

        var trimmed = input.Trim();
        if (trimmed.Length > 128)
            throw new ArgumentException("Device id cannot exceed 128 characters.", nameof(input));

        return new DeviceId(trimmed);
    }

    public override string ToString() => Value;
}
