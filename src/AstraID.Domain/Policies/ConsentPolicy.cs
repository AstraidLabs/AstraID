using AstraID.Domain.Constants;
using AstraID.Domain.Entities;
using AstraID.Domain.Results;
using AstraID.Domain.Errors;

namespace AstraID.Domain.Policies;

/// <summary>Validates user consent grants.</summary>
public sealed class ConsentPolicy
{
    /// <summary>Validates that the grant is allowed.</summary>
    public Result ValidateGrant(Client client, IReadOnlyCollection<Scope> scopes)
    {
        if (scopes == null || scopes.Count == 0)
            return Result.Failure(DomainError.From(DomainErrorCodes.InvalidScope, "Scopes cannot be empty."));

        foreach (var scope in scopes)
        {
            if (!ScopeNames.All.Contains(scope.Value))
                return Result.Failure(DomainError.From(DomainErrorCodes.InvalidScope, $"Invalid scope '{scope.Value}'."));

            if (client.Type == ClientType.Public && scope.Value == ScopeNames.OfflineAccess)
                return Result.Failure(DomainError.From(DomainErrorCodes.InvalidScope, "Public clients cannot request offline access."));
        }

        return Result.Success();
    }
}
