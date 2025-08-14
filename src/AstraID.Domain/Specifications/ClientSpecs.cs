using System.Linq;
using System.Linq.Expressions;
using AstraID.Domain.Entities;

namespace AstraID.Domain.Specifications;

/// <summary>Specifications for querying clients.</summary>
public static class ClientSpecs
{
    /// <summary>Filters by client identifier and tenant.</summary>
    public static Expression<Func<Client, bool>> ByClientId(string clientId, Guid? tenantId)
        => c => c.ClientId == clientId && c.TenantId == tenantId;

    /// <summary>Filters clients that allow the given scope.</summary>
    public static Expression<Func<Client, bool>> ByScope(string scope)
        => c => c.Scopes.Any(s => s.Value == scope);

    /// <summary>Filters by tenant.</summary>
    public static Expression<Func<Client, bool>> ByTenant(Guid? tenantId)
        => c => c.TenantId == tenantId;
}
