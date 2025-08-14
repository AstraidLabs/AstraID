using System.Linq.Expressions;
using AstraID.Domain.Entities;

namespace AstraID.Domain.Specifications;

/// <summary>Specifications for querying consents.</summary>
public static class ConsentSpecs
{
    /// <summary>Consent for given user and client.</summary>
    public static Expression<Func<UserConsent, bool>> ByUserAndClient(Guid userId, string clientId)
        => c => c.UserId == userId && c.ClientId == clientId;

    /// <summary>Only active consents.</summary>
    public static Expression<Func<UserConsent, bool>> ActiveOnly()
        => c => c.RevokedUtc == null;
}
