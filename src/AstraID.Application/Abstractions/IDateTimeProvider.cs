namespace AstraID.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
