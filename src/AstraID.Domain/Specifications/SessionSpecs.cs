using System.Linq.Expressions;
using AstraID.Domain.Entities;

namespace AstraID.Domain.Specifications;

/// <summary>Specifications for querying sessions.</summary>
public static class SessionSpecs
{
    /// <summary>Active sessions for a user.</summary>
    public static Expression<Func<UserSession, bool>> ActiveByUser(Guid userId)
        => s => s.UserId == userId && s.IsActive;

    /// <summary>Session by device.</summary>
    public static Expression<Func<UserSession, bool>> ByDevice(Guid userId, string deviceId)
        => s => s.UserId == userId && s.DeviceId.Value == deviceId && s.IsActive;

    /// <summary>All sessions for a user.</summary>
    public static Expression<Func<UserSession, bool>> ByUser(Guid userId)
        => s => s.UserId == userId;
}
