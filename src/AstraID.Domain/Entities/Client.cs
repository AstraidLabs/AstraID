using System.Linq;
using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents an OIDC/OAuth client application.
/// </summary>
public sealed class Client : AggregateRoot<Guid>
{
    private readonly HashSet<Scope> _scopes = new();
    private readonly HashSet<RedirectUri> _redirectUris = new();
    private readonly HashSet<RedirectUri> _postLogoutRedirectUris = new();
    private readonly List<ClientSecretHistory> _secretHistory = new();
    private readonly HashSet<ClientCorsOrigin> _corsOrigins = new();

    /// <summary>
    /// Client identifier string.
    /// </summary>
    public string ClientId { get; private set; } = string.Empty;

    /// <summary>
    /// Human friendly display name.
    /// </summary>
    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>
    /// Client type (public or confidential).
    /// </summary>
    public ClientType Type { get; private set; }

    /// <summary>
    /// Current hashed secret if confidential.
    /// </summary>
    public HashedSecret? ClientSecretHash { get; private set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedUtc { get; private set; }

    /// <summary>
    /// Identifier of creator, if any.
    /// </summary>
    public Guid? CreatedBy { get; private set; }

    /// <summary>
    /// Optional tenant identifier.
    /// </summary>
    public Guid? TenantId { get; private set; }

    /// <summary>
    /// Allowed scopes.
    /// </summary>
    public IReadOnlyCollection<Scope> Scopes => _scopes;

    /// <summary>
    /// Redirect URIs.
    /// </summary>
    public IReadOnlyCollection<RedirectUri> RedirectUris => _redirectUris;

    /// <summary>
    /// Post logout redirect URIs.
    /// </summary>
    public IReadOnlyCollection<RedirectUri> PostLogoutRedirectUris => _postLogoutRedirectUris;

    /// <summary>
    /// Secret history entries.
    /// </summary>
    public IReadOnlyCollection<ClientSecretHistory> SecretHistory => _secretHistory;

    /// <summary>
    /// Allowed CORS origins.
    /// </summary>
    public IReadOnlyCollection<ClientCorsOrigin> CorsOrigins => _corsOrigins;

    private Client()
    {
    }

    private Client(string clientId, string displayName, ClientType type, IEnumerable<Scope>? scopes,
        IEnumerable<RedirectUri>? redirectUris, IEnumerable<RedirectUri>? postLogoutUris,
        Guid? createdBy, Guid? tenantId) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client identifier cannot be empty.", nameof(clientId));

        ClientId = clientId.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? ClientId : displayName.Trim();
        Type = type;
        CreatedUtc = DateTime.UtcNow;
        CreatedBy = createdBy;
        TenantId = tenantId;

        if (scopes != null)
            foreach (var s in scopes)
                _scopes.Add(s);

        if (redirectUris != null)
            foreach (var r in redirectUris)
                _redirectUris.Add(r);

        if (postLogoutUris != null)
            foreach (var r in postLogoutUris)
                _postLogoutRedirectUris.Add(r);

        Raise(new ClientRegistered(Id, ClientId));
    }

    /// <summary>
    /// Registers a new client aggregate and raises <see cref="ClientRegistered"/>.
    /// </summary>
    public static Client Register(string clientId, string displayName, ClientType type,
        IEnumerable<Scope>? scopes = null,
        IEnumerable<RedirectUri>? redirectUris = null,
        IEnumerable<RedirectUri>? postLogoutUris = null,
        IEnumerable<ClientCorsOrigin>? corsOrigins = null,
        Guid? createdBy = null,
        Guid? tenantId = null)
    {
        var client = new Client(clientId, displayName, type, scopes, redirectUris, postLogoutUris, createdBy, tenantId);

        if (corsOrigins != null)
            foreach (var o in corsOrigins)
                client.AddCorsOrigin(o);

        return client;
    }

    /// <summary>
    /// Allows a scope for the client.
    /// </summary>
    public void AllowScope(Scope scope)
    {
        if (_scopes.Add(scope))
            Touch();
    }

    /// <summary>
    /// Removes a scope by name.
    /// </summary>
    public void RemoveScope(string name)
    {
        _scopes.RemoveWhere(s => s.Value == name.ToLowerInvariant());
        Touch();
    }

    /// <summary>
    /// Adds a redirect URI.
    /// </summary>
    public void AddRedirect(RedirectUri uri)
    {
        if (_redirectUris.Add(uri))
            Touch();
    }

    /// <summary>
    /// Removes a redirect URI.
    /// </summary>
    public void RemoveRedirect(string uri)
    {
        _redirectUris.Remove(RedirectUri.Create(uri));
        Touch();
    }

    /// <summary>
    /// Adds a post-logout redirect URI.
    /// </summary>
    public void AddPostLogoutRedirect(RedirectUri uri)
    {
        if (_postLogoutRedirectUris.Add(uri))
            Touch();
    }

    /// <summary>
    /// Removes a post-logout redirect URI.
    /// </summary>
    public void RemovePostLogoutRedirect(string uri)
    {
        _postLogoutRedirectUris.Remove(RedirectUri.Create(uri));
        Touch();
    }

    /// <summary>
    /// Adds a CORS origin.
    /// </summary>
    public void AddCorsOrigin(ClientCorsOrigin origin)
    {
        if (origin.ClientId != Id)
            origin = ClientCorsOrigin.Create(Id, origin.Origin);

        if (_corsOrigins.Add(origin))
            Touch();
    }

    /// <summary>
    /// Removes a CORS origin.
    /// </summary>
    public void RemoveCorsOrigin(string origin)
    {
        _corsOrigins.Remove(ClientCorsOrigin.Create(Id, origin));
        Touch();
    }

    /// <summary>
    /// Rotates the client secret. Only valid for confidential clients.
    /// </summary>
    public void RotateSecret(HashedSecret newHash)
    {
        if (Type != ClientType.Confidential)
            throw new InvalidOperationException("Public clients cannot have secrets.");

        foreach (var entry in _secretHistory.Where(h => h.Active))
            entry.Deactivate();

        var history = ClientSecretHistory.Create(Id, newHash, true);
        _secretHistory.Add(history);
        ClientSecretHash = newHash;
        Raise(new ClientSecretRotated(Id));
        Touch();
    }
}

/// <summary>
/// Type of client application.
/// </summary>
public enum ClientType
{
    /// <summary>Public client without a secret.</summary>
    Public,
    /// <summary>Confidential client with a secret.</summary>
    Confidential
}
