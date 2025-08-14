using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using AstraID.Domain.Errors;
using AstraID.Domain.Policies;
using AstraID.Domain.Repositories;
using AstraID.Domain.Results;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Services;

/// <summary>Coordinates user-related operations.</summary>
public sealed class UserDomainService
{
    private readonly IAppUserRepository _users;
    private readonly IPasswordHistoryRepository _passwordHistory;
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionRepository _sessions;
    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly LockoutPolicy _lockoutPolicy;
    private readonly TwoFactorPolicy _twoFactorPolicy;
    private readonly IDateTimeProvider _clock;

    /// <summary>Initializes the service.</summary>
    public UserDomainService(
        IAppUserRepository users,
        IPasswordHistoryRepository passwordHistory,
        IPasswordPolicy passwordPolicy,
        IPasswordHasher passwordHasher,
        IUserSessionRepository sessions,
        IUnitOfWork uow,
        IDomainEventDispatcher dispatcher,
        LockoutPolicy lockoutPolicy,
        TwoFactorPolicy twoFactorPolicy,
        IDateTimeProvider clock)
    {
        _users = users;
        _passwordHistory = passwordHistory;
        _passwordPolicy = passwordPolicy;
        _passwordHasher = passwordHasher;
        _sessions = sessions;
        _uow = uow;
        _dispatcher = dispatcher;
        _lockoutPolicy = lockoutPolicy;
        _twoFactorPolicy = twoFactorPolicy;
        _clock = clock;
    }

    /// <summary>Registers a new user.</summary>
    public async Task<Result<AppUser>> RegisterAsync(Email email, DisplayName displayName, string passwordPlain, Guid? tenantId, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(email.Value.ToUpperInvariant(), ct))
            return Result<AppUser>.Failure(DomainError.From(DomainErrorCodes.EmailAlreadyExists, "Email already exists."));

        var strengthError = _passwordPolicy.ValidateStrength(passwordPlain);
        if (strengthError != null)
            return Result<AppUser>.Failure(DomainError.From(DomainErrorCodes.WeakPassword, strengthError));

        var user = AppUser.Register(email, displayName, tenantId);
        user.PasswordHash = _passwordHasher.Hash(passwordPlain);
        await _users.AddAsync(user, ct);
        await _passwordHistory.AddAsync(PasswordHistory.Record(user.Id, user.PasswordHash), ct);
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result<AppUser>.Success(user);
    }

    /// <summary>Changes user email.</summary>
    public async Task<Result> ChangeEmailAsync(Guid userId, Email newEmail, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            return Result.Failure(DomainError.From("user.notFound", "User not found."));
        if (!user.IsActive)
            return Result.Failure(DomainError.From("user.inactive", "User is inactive."));
        if (await _users.ExistsByEmailAsync(newEmail.Value.ToUpperInvariant(), ct))
            return Result.Failure(DomainError.From(DomainErrorCodes.EmailAlreadyExists, "Email already exists."));

        user.ChangeEmail(newEmail);
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result.Success();
    }

    /// <summary>Enables two-factor authentication for the user.</summary>
    public async Task<Result> EnableTwoFactorAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            return Result.Failure(DomainError.From("user.notFound", "User not found."));

        var policyResult = _twoFactorPolicy.CanEnable(user);
        if (!policyResult.IsSuccess)
            return policyResult;

        user.EnableTwoFactor();
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result.Success();
    }

    /// <summary>Disables two-factor authentication for the user.</summary>
    public async Task<Result> DisableTwoFactorAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            return Result.Failure(DomainError.From("user.notFound", "User not found."));

        var policyResult = _twoFactorPolicy.CanDisable(false, true);
        if (!policyResult.IsSuccess)
            return policyResult;

        user.DisableTwoFactor();
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result.Success();
    }

    /// <summary>Records a successful login.</summary>
    public async Task<Result> RecordLoginSuccessAsync(Guid userId, DeviceId device, IpAddress ip, UserAgent agent, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            return Result.Failure(DomainError.From("user.notFound", "User not found."));

        user.RecordSuccessfulLogin(_clock.UtcNow);
        var session = await _sessions.GetActiveByDeviceAsync(userId, device.Value, ct);
        if (session != null)
        {
            session.SeenNow();
        }
        else
        {
            await _sessions.AddAsync(UserSession.Start(userId, device, ip, agent, user.TenantId), ct);
        }

        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result.Success();
    }

    /// <summary>Records a failed login attempt.</summary>
    public async Task<Result> RecordLoginFailureAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            return Result.Failure(DomainError.From("user.notFound", "User not found."));

        user.RecordFailedLogin();
        if (_lockoutPolicy.ShouldLock(user.AccessFailedCount, 5, 15, out var duration))
            user.LockoutEnd = DateTimeOffset.UtcNow.Add(duration);

        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(user.DomainEvents, ct);
        user.ClearDomainEvents();
        return Result.Success();
    }
}
