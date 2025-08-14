namespace AstraID.Domain.Abstractions;

/// <summary>Provides the current UTC time.</summary>
public interface IDateTimeProvider
{
    /// <summary>Returns UTC now.</summary>
    DateTime UtcNow { get; }
}
