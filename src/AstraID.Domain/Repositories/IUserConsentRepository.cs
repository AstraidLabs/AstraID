using System.Collections.Generic;
using AstraID.Domain.Entities;

namespace AstraID.Domain.Repositories;

/// <summary>Repository for <see cref="UserConsent"/> aggregates.</summary>
public interface IUserConsentRepository
{
    /// <summary>Gets consent by user and client.</summary>
    Task<UserConsent?> GetByUserAndClientAsync(Guid userId, string clientId, CancellationToken ct = default);

    /// <summary>Adds a new consent.</summary>
    Task AddAsync(UserConsent consent, CancellationToken ct = default);

    /// <summary>Lists active consents for a user.</summary>
    Task<IReadOnlyList<UserConsent>> ListByUserAsync(Guid userId, CancellationToken ct = default);
}
