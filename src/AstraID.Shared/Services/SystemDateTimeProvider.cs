using AstraID.Application.Abstractions;
using DomainDateTimeProvider = AstraID.Domain.Abstractions.IDateTimeProvider;

namespace AstraID.Shared.Services;

/// <summary>Provides the current system time.</summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider, DomainDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <inheritdoc />
    DateTime DomainDateTimeProvider.UtcNow => DateTime.UtcNow;
}
