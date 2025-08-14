using AstraID.Domain.Primitives;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents an allowed CORS origin for a client.
/// </summary>
public sealed class ClientCorsOrigin : Entity<Guid>
{
    /// <summary>
    /// Owning client identifier.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Canonical origin string.
    /// </summary>
    public string Origin { get; private set; } = string.Empty;

    private ClientCorsOrigin()
    {
    }

    private ClientCorsOrigin(Guid clientId, string origin) : base(Guid.NewGuid())
    {
        ClientId = clientId;
        Origin = Normalize(origin);
    }

    /// <summary>
    /// Creates a new origin entry.
    /// </summary>
    public static ClientCorsOrigin Create(Guid clientId, string origin) => new(clientId, origin);

    private static string Normalize(string origin)
    {
        if (!Uri.TryCreate(origin.Trim(), UriKind.Absolute, out var uri))
            throw new ArgumentException("Origin must be an absolute URI.", nameof(origin));
        if (!string.IsNullOrEmpty(uri.PathAndQuery) && uri.PathAndQuery != "/")
            throw new ArgumentException("Origin must not contain a path.", nameof(origin));
        return $"{uri.Scheme}://{uri.Authority}".ToLowerInvariant();
    }

    public override bool Equals(object? obj)
        => obj is ClientCorsOrigin other && Origin.Equals(other.Origin, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => Origin.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
