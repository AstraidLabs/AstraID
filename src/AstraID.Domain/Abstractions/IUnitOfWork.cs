namespace AstraID.Domain.Abstractions;

/// <summary>Represents a unit of work for persisting changes.</summary>
public interface IUnitOfWork
{
    /// <summary>Atomically persists changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
