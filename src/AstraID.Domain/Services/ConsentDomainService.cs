using System.Linq;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using AstraID.Domain.Errors;
using AstraID.Domain.Policies;
using AstraID.Domain.Repositories;
using AstraID.Domain.Results;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Services;

/// <summary>Coordinates consent operations.</summary>
public sealed class ConsentDomainService
{
    private readonly IUserConsentRepository _consents;
    private readonly IClientRepository _clients;
    private readonly ConsentPolicy _policy;
    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>Initializes the service.</summary>
    public ConsentDomainService(IUserConsentRepository consents, IClientRepository clients, ConsentPolicy policy, IUnitOfWork uow, IDomainEventDispatcher dispatcher)
    {
        _consents = consents;
        _clients = clients;
        _policy = policy;
        _uow = uow;
        _dispatcher = dispatcher;
    }

    /// <summary>Grants consent for the specified scopes.</summary>
    public async Task<Result<UserConsent>> GrantAsync(Guid userId, string clientId, IEnumerable<Scope> scopes, CancellationToken ct)
    {
        var client = await _clients.GetByClientIdAsync(clientId, null, ct);
        if (client == null)
            return Result<UserConsent>.Failure(DomainError.From("client.notFound", "Client not found."));

        var scopeList = scopes.ToList();
        var validation = _policy.ValidateGrant(client, scopeList);
        if (!validation.IsSuccess)
            return Result<UserConsent>.Failure(validation.Error!);

        var consent = await _consents.GetByUserAndClientAsync(userId, clientId, ct);
        if (consent == null)
        {
            consent = UserConsent.Grant(userId, clientId, scopeList, client.TenantId);
            await _consents.AddAsync(consent, ct);
        }
        else
        {
            consent.UpdateScopes(scopeList);
        }

        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(consent.DomainEvents, ct);
        consent.ClearDomainEvents();
        return Result<UserConsent>.Success(consent);
    }

    /// <summary>Revokes consent for the user and client.</summary>
    public async Task<Result> RevokeAsync(Guid userId, string clientId, CancellationToken ct)
    {
        var consent = await _consents.GetByUserAndClientAsync(userId, clientId, ct);
        if (consent == null)
            return Result.Success();

        consent.Revoke();
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(consent.DomainEvents, ct);
        consent.ClearDomainEvents();
        return Result.Success();
    }
}
