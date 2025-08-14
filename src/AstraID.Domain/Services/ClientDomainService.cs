using System.Linq;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using AstraID.Domain.Errors;
using AstraID.Domain.Policies;
using AstraID.Domain.Repositories;
using AstraID.Domain.Results;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Services;

/// <summary>Coordinates client operations.</summary>
public sealed class ClientDomainService
{
    private readonly IClientRepository _clients;
    private readonly IPasswordHasher _hasher;
    private readonly ClientSecretPolicy _secretPolicy;
    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>Initializes the service.</summary>
    public ClientDomainService(IClientRepository clients, IPasswordHasher hasher, ClientSecretPolicy secretPolicy, IUnitOfWork uow, IDomainEventDispatcher dispatcher)
    {
        _clients = clients;
        _hasher = hasher;
        _secretPolicy = secretPolicy;
        _uow = uow;
        _dispatcher = dispatcher;
    }

    /// <summary>Registers a new client.</summary>
    public async Task<Result<Client>> RegisterAsync(string clientId, string displayName, ClientType type, IEnumerable<Scope> scopes, IEnumerable<RedirectUri> redirects, IEnumerable<RedirectUri> postLogout, IEnumerable<string> corsOrigins, Guid? tenantId, Guid? createdBy, CancellationToken ct)
    {
        if (await _clients.GetByClientIdAsync(clientId, tenantId, ct) != null)
            return Result<Client>.Failure(DomainError.From(DomainErrorCodes.ClientExists, "Client already exists."));

        var client = Client.Register(clientId, displayName, type, scopes, redirects, postLogout, null, createdBy, tenantId);
        if (corsOrigins != null)
        {
            foreach (var o in corsOrigins)
                client.AddCorsOrigin(ClientCorsOrigin.Create(client.Id, o));
        }

        await _clients.AddAsync(client, ct);
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(client.DomainEvents, ct);
        client.ClearDomainEvents();
        return Result<Client>.Success(client);
    }

    /// <summary>Rotates a confidential client secret.</summary>
    public async Task<Result> RotateSecretAsync(string clientId, string newPlainSecret, CancellationToken ct)
    {
        var client = await _clients.GetByClientIdAsync(clientId, null, ct);
        if (client == null)
            return Result.Failure(DomainError.From("client.notFound", "Client not found."));
        if (client.Type != ClientType.Confidential)
            return Result.Failure(DomainError.From("client.public", "Public clients cannot rotate secrets."));

        var strength = _secretPolicy.ValidateSecretPlaintext(newPlainSecret);
        if (!strength.IsSuccess)
            return strength;

        var rotation = _secretPolicy.ValidateRotation(client, DateTime.UtcNow);
        if (!rotation.IsSuccess)
            return rotation;

        client.RotateSecret(HashedSecret.FromHash(_hasher.Hash(newPlainSecret)));
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(client.DomainEvents, ct);
        client.ClearDomainEvents();
        return Result.Success();
    }
}
