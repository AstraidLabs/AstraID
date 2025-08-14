using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Text.Json;

namespace AstraID.Infrastructure.OpenIddict;

/// <summary>
/// Bridges client aggregate changes to OpenIddict applications.
/// </summary>
public sealed class ClientApplicationBridge : IClientApplicationBridge
{
    private readonly IOpenIddictApplicationManager _apps;

    public ClientApplicationBridge(IOpenIddictApplicationManager apps) => _apps = apps;

    /// <inheritdoc />
    public async Task EnsureCreatedAsync(Client client, CancellationToken ct = default)
    {
        var existing = await _apps.FindByClientIdAsync(client.ClientId, ct).ConfigureAwait(false);
        if (existing is not null)
            return;

        var descriptor = CreateDescriptor(client);
        await _apps.CreateAsync(descriptor, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ApplyChangesAsync(Client client, CancellationToken ct = default)
    {
        var app = await _apps.FindByClientIdAsync(client.ClientId, ct).ConfigureAwait(false);
        if (app is null)
        {
            await EnsureCreatedAsync(client, ct).ConfigureAwait(false);
            return;
        }

        var descriptor = new OpenIddictApplicationDescriptor();
        await _apps.PopulateAsync(descriptor, app, ct).ConfigureAwait(false);

        PopulateDescriptor(descriptor, client);
        await _apps.UpdateAsync(app, descriptor, ct).ConfigureAwait(false);
    }

    private static OpenIddictApplicationDescriptor CreateDescriptor(Client client)
    {
        var descriptor = new OpenIddictApplicationDescriptor();
        PopulateDescriptor(descriptor, client);
        return descriptor;
    }

    private static void PopulateDescriptor(OpenIddictApplicationDescriptor descriptor, Client client)
    {
        descriptor.ClientId = client.ClientId;
        descriptor.DisplayName = client.DisplayName;
        descriptor.ClientType = client.Type == ClientType.Public
            ? OpenIddictConstants.ClientTypes.Public
            : OpenIddictConstants.ClientTypes.Confidential;

        descriptor.ClientSecret = client.ClientSecretHashRaw;

        descriptor.RedirectUris.Clear();
        foreach (var uri in client.RedirectUris)
            descriptor.RedirectUris.Add(new Uri(uri.Value));

        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var uri in client.PostLogoutRedirectUris)
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri.Value));

        descriptor.Permissions.Clear();
        foreach (var scope in client.Scopes)
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope.Value);

        if (client.CorsOrigins.Any())
            descriptor.Properties["cors_origins"] = JsonSerializer.SerializeToElement(
                string.Join(";", client.CorsOrigins.Select(o => o.Origin)));
    }
}
