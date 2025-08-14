namespace AstraID.Domain.Abstractions;

using AstraID.Domain.Entities;

/// <summary>
/// Synchronizes client aggregates with OpenIddict applications.
/// </summary>
public interface IClientApplicationBridge
{
    /// <summary>
    /// Ensures that an OpenIddict application exists for the specified client.
    /// </summary>
    Task EnsureCreatedAsync(Client client, CancellationToken ct = default);

    /// <summary>
    /// Applies changes from the client aggregate to the corresponding OpenIddict application.
    /// </summary>
    Task ApplyChangesAsync(Client client, CancellationToken ct = default);
}
