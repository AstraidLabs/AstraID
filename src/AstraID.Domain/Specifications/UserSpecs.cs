using System.Linq.Expressions;
using AstraID.Domain.Entities;

namespace AstraID.Domain.Specifications;

/// <summary>Specifications for querying users.</summary>
public static class UserSpecs
{
    /// <summary>Filters by normalized email.</summary>
    public static Expression<Func<AppUser, bool>> ByEmail(string normalizedEmail)
        => u => u.NormalizedEmail == normalizedEmail;

    /// <summary>Filters active users.</summary>
    public static Expression<Func<AppUser, bool>> ActiveOnly()
        => u => u.IsActive;

    /// <summary>Filters by tenant.</summary>
    public static Expression<Func<AppUser, bool>> ByTenant(Guid? tenantId)
        => u => u.TenantId == tenantId;
}
