namespace AstraID.Domain.ValueObjects;

/// <summary>
/// Represents a redirect URI.
/// </summary>
public sealed record RedirectUri
{
    /// <summary>
    /// Gets the canonical URI value.
    /// </summary>
    public string Value { get; }

    private RedirectUri(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="RedirectUri"/> after validation.
    /// </summary>
    public static RedirectUri Create(string input)
    {
        if (!Uri.TryCreate(input?.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Redirect URI must be an absolute HTTP/HTTPS URI.", nameof(input));

        return new RedirectUri(uri.ToString());
    }

    public override string ToString() => Value;
}
