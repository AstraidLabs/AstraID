using AstraID.Domain.Entities;
using AstraID.Domain.Errors;
using AstraID.Domain.Results;

namespace AstraID.Domain.Policies;

/// <summary>Rules for enabling and disabling two-factor authentication.</summary>
public sealed class TwoFactorPolicy
{
    /// <summary>Determines whether two-factor can be enabled.</summary>
    public Result CanEnable(AppUser user)
    {
        if (!user.IsActive)
            return Result.Failure(DomainError.From("user.inactive", "Inactive users cannot enable two-factor."));
        if (user.TwoFactorEnabled)
            return Result.Failure(DomainError.From("twofactor.enabled", "Two-factor already enabled."));
        return Result.Success();
    }

    /// <summary>Determines whether two-factor can be disabled.</summary>
    public Result CanDisable(bool tenantRequiresTwoFactor, bool hasRecoveryCodes)
    {
        if (tenantRequiresTwoFactor)
            return Result.Failure(DomainError.From("twofactor.required", "Tenant requires two-factor."));
        if (!hasRecoveryCodes)
            return Result.Failure(DomainError.From("twofactor.recovery", "Recovery codes exhausted."));
        return Result.Success();
    }
}
