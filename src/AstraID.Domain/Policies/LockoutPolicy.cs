namespace AstraID.Domain.Policies;

/// <summary>Determines when a user should be locked out after failed logins.</summary>
public sealed class LockoutPolicy
{
    /// <summary>Determines whether lockout should occur.</summary>
    public bool ShouldLock(int failedCount, int maxFailedAttempts, int lockoutMinutes, out TimeSpan duration)
    {
        if (failedCount >= maxFailedAttempts)
        {
            duration = TimeSpan.FromMinutes(lockoutMinutes);
            return true;
        }

        duration = TimeSpan.Zero;
        return false;
    }
}
