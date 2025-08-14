using AstraID.Domain.Entities;
using OpenIddict.Abstractions;

namespace AstraID.Infrastructure.OpenIddict;

/// <summary>
/// Bridges client aggregate changes to OpenIddict applications.
/// </summary>
public class ClientApplicationBridge
{
    private readonly IOpenIddictApplicationManager _apps;

    public ClientApplicationBridge(IOpenIddictApplicationManager apps) => _apps = apps;

    public async Task EnsureCreatedAsync(Client client, CancellationToken ct = default)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = client.ClientId,
            DisplayName = client.DisplayName,
            Type = client.Type
        };
        await _apps.CreateAsync(descriptor, ct);
    }

    public async Task ApplyChangesAsync(Client client, CancellationToken ct = default)
    {
        var app = await _apps.FindByClientIdAsync(client.ClientId, ct);
        if (app == null)
        {
            await EnsureCreatedAsync(client, ct);
            return;
        }
        var descriptor = new OpenIddictApplicationDescriptor();
        await _apps.PopulateAsync(descriptor, app, ct);
        descriptor.DisplayName = client.DisplayName;
        descriptor.Type = client.Type;
        await _apps.UpdateAsync(app, descriptor, ct);
    }
}
