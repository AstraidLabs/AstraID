using System.Net.Mail;

namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Email address value object.
/// </summary>
public sealed record Email
{
    /// <summary>
    /// Gets the normalized email value.
    /// </summary>
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="Email"/> instance after validation.
    /// </summary>
    public static Email Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Email cannot be empty.", nameof(input));

        var address = new MailAddress(input.Trim());
        return new Email(address.Address.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
