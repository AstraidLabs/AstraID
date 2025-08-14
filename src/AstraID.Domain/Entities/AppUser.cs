using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Domain.Entities;

[Table("Users", Schema = "auth")]
[Index(nameof(NormalizedEmail))]
[Index(nameof(IsActive))]
/// <summary>
/// Application user aggregate root.
/// </summary>
public sealed class AppUser : IdentityUser<Guid>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets domain events raised by this user.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Raw display name value.
    /// </summary>
    [MaxLength(128)]
    public string? DisplayNameRaw { get; private set; }

    /// <summary>
    /// Indicates whether the user is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// UTC timestamp when the user was created.
    /// </summary>
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Last successful login time in UTC.
    /// </summary>
    public DateTime? LastLoginUtc { get; private set; }

    /// <summary>
    /// Optional tenant identifier for multi-tenancy.
    /// </summary>
    public Guid? TenantId { get; private set; }

    [ConcurrencyCheck]
    public override string? ConcurrencyStamp { get; set; }

    private AppUser()
    {
    }

    private AppUser(Email email, DisplayName displayName, Guid? tenantId)
    {
        Id = Guid.NewGuid();
        UserName = email.Value;
        NormalizedUserName = email.Value.ToUpperInvariant();
        Email = email.Value;
        NormalizedEmail = email.Value.ToUpperInvariant();
        EmailConfirmed = false;
        DisplayNameRaw = displayName.Value;
        TenantId = tenantId;
        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
        Raise(new UserRegistered(Id, email.Value));
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public static AppUser Register(Email email, DisplayName displayName, Guid? tenantId = null)
        => new(email, displayName, tenantId);

    /// <summary>
    /// Marks the email as confirmed.
    /// </summary>
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        Touch();
    }

    /// <summary>
    /// Changes the user's email. Only allowed for active users.
    /// </summary>
    public void ChangeEmail(Email newEmail)
    {
        if (!IsActive)
            throw new InvalidOperationException("Inactive users cannot change email.");

        Email = newEmail.Value;
        UserName = newEmail.Value;
        NormalizedEmail = newEmail.Value.ToUpperInvariant();
        NormalizedUserName = newEmail.Value.ToUpperInvariant();
        EmailConfirmed = false;
        Touch();
    }

    /// <summary>
    /// Changes the display name.
    /// </summary>
    public void ChangeDisplayName(DisplayName displayName)
    {
        DisplayNameRaw = displayName.Value;
        Touch();
    }

    /// <summary>
    /// Records a successful login and resets failure counters.
    /// </summary>
    public void RecordSuccessfulLogin(DateTime? whenUtc = null)
    {
        LastLoginUtc = whenUtc ?? DateTime.UtcNow;
        AccessFailedCount = 0;
        LockoutEnd = null;
        Touch();
    }

    /// <summary>
    /// Records a failed login attempt.
    /// </summary>
    public void RecordFailedLogin()
    {
        AccessFailedCount++;
        Touch();
    }

    /// <summary>
    /// Enables two-factor authentication and raises an event.
    /// </summary>
    public void EnableTwoFactor()
    {
        if (!TwoFactorEnabled)
        {
            TwoFactorEnabled = true;
            Raise(new TwoFactorEnabled(Id));
            Touch();
        }
    }

    /// <summary>
    /// Disables two-factor authentication.
    /// </summary>
    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        Touch();
    }

    /// <summary>
    /// Deactivates the user and raises a <see cref="UserLocked"/> event.
    /// </summary>
    public void Deactivate(string reason = "deactivated")
    {
        if (!IsActive)
            return;

        IsActive = false;
        LockoutEnabled = true;
        LockoutEnd = DateTimeOffset.MaxValue;
        Raise(new UserLocked(Id, reason));
        Touch();
    }

    /// <summary>
    /// Activates a previously deactivated user.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LockoutEnabled = false;
        LockoutEnd = null;
        Touch();
    }

    /// <summary>
    /// Records a password change.
    /// </summary>
    public void ChangePassword()
    {
        Raise(new PasswordChanged(Id));
        Touch();
    }

    /// <summary>
    /// Requests a password reset, raising an event.
    /// </summary>
    public void RequestPasswordReset()
    {
        Raise(new PasswordResetRequested(Id));
    }

    private void Touch() => ConcurrencyStamp = Guid.NewGuid().ToString("N");

    private void Raise(IDomainEvent evt) => _domainEvents.Add(evt);

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
