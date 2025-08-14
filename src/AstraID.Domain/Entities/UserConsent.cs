using System.Linq;
using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents a user's consent for a client application.
/// </summary>
public sealed class UserConsent : AggregateRoot<Guid>
{
    private readonly HashSet<Scope> _scopes = new();

    /// <summary>
    /// User identifier granting consent.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Client identifier receiving consent.
    /// </summary>
    public string ClientId { get; private set; } = string.Empty;

    /// <summary>
    /// Optional tenant identifier.
    /// </summary>
    public Guid? TenantId { get; private set; }

    /// <summary>
    /// Timestamp when consent was granted.
    /// </summary>
    public DateTime GrantedUtc { get; private set; }

    /// <summary>
    /// Revocation timestamp if consent was revoked.
    /// </summary>
    public DateTime? RevokedUtc { get; private set; }

    /// <summary>
    /// Scopes granted to the client.
    /// </summary>
    public IReadOnlyCollection<Scope> Scopes => _scopes;

    private UserConsent()
    {
    }

    private UserConsent(Guid userId, string clientId, IEnumerable<Scope> scopes, Guid? tenantId) : base(Guid.NewGuid())
    {
        var list = scopes?.ToList() ?? new List<Scope>();
        if (list.Count == 0)
            throw new ArgumentException("Consent must contain at least one scope.", nameof(scopes));

        UserId = userId;
        ClientId = clientId;
        TenantId = tenantId;
        foreach (var s in list)
            _scopes.Add(s);

        GrantedUtc = DateTime.UtcNow;
        Raise(new ConsentGranted(userId, clientId, list.Select(s => s.Value).ToArray()));
    }

    /// <summary>
    /// Grants consent for the specified scopes.
    /// </summary>
    public static UserConsent Grant(Guid userId, string clientId, IEnumerable<Scope> scopes, Guid? tenantId = null)
        => new(userId, clientId, scopes, tenantId);

    /// <summary>
    /// Revokes the consent.
    /// </summary>
    public void Revoke()
    {
        if (RevokedUtc != null)
            throw new InvalidOperationException("Consent already revoked.");

        RevokedUtc = DateTime.UtcNow;
        Raise(new ConsentRevoked(UserId, ClientId));
        Touch();
    }

    /// <summary>
    /// Updates the set of scopes granted.
    /// </summary>
    public void UpdateScopes(IEnumerable<Scope> scopes)
    {
        if (RevokedUtc != null)
            throw new InvalidOperationException("Cannot update revoked consent.");

        var list = scopes?.ToList() ?? new List<Scope>();
        if (list.Count == 0)
            throw new ArgumentException("Scopes cannot be empty.", nameof(scopes));

        _scopes.Clear();
        foreach (var s in list)
            _scopes.Add(s);

        Touch();
    }
}
