using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using AstraID.Domain.Repositories;
using AstraID.Domain.Results;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Services;

/// <summary>Coordinates session operations.</summary>
public sealed class SessionDomainService
{
    private readonly IUserSessionRepository _sessions;
    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IDateTimeProvider _clock;

    /// <summary>Initializes the service.</summary>
    public SessionDomainService(IUserSessionRepository sessions, IUnitOfWork uow, IDomainEventDispatcher dispatcher, IDateTimeProvider clock)
    {
        _sessions = sessions;
        _uow = uow;
        _dispatcher = dispatcher;
        _clock = clock;
    }

    /// <summary>Starts a new session.</summary>
    public async Task<Result<UserSession>> StartAsync(Guid userId, DeviceId deviceId, IpAddress ip, UserAgent agent, CancellationToken ct)
    {
        var existing = await _sessions.GetActiveByDeviceAsync(userId, deviceId.Value, ct);
        if (existing != null)
        {
            existing.SeenNow();
            await _uow.SaveChangesAsync(ct);
            await _dispatcher.DispatchAsync(existing.DomainEvents, ct);
            existing.ClearDomainEvents();
            return Result<UserSession>.Success(existing);
        }

        var session = UserSession.Start(userId, deviceId, ip, agent, null);
        await _sessions.AddAsync(session, ct);
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(session.DomainEvents, ct);
        session.ClearDomainEvents();
        return Result<UserSession>.Success(session);
    }

    /// <summary>Revokes a specific session.</summary>
    public async Task<Result> RevokeAsync(Guid userId, string deviceId, string reason, CancellationToken ct)
    {
        var session = await _sessions.GetActiveByDeviceAsync(userId, deviceId, ct);
        if (session == null)
            return Result.Success();
        session.Revoke(reason);
        await _uow.SaveChangesAsync(ct);
        await _dispatcher.DispatchAsync(session.DomainEvents, ct);
        session.ClearDomainEvents();
        return Result.Success();
    }

    /// <summary>Revokes all sessions for a user.</summary>
    public Task<int> RevokeAllAsync(Guid userId, string reason, CancellationToken ct)
        => _sessions.RevokeAllAsync(userId, reason, _clock.UtcNow, ct);
}
