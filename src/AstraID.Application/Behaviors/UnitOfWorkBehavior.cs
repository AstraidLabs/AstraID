using AstraID.Domain.Abstractions;
using MediatR;

namespace AstraID.Application.Behaviors;

/// <summary>
/// Commits all changes to the database after the request has been handled.
/// </summary>
/// <remarks>
/// Executes late in the MediatR pipeline so all preceding behaviors (validation,
/// authorization) have succeeded. Ensures a single transaction per request for
/// consistency and minimal lock duration.
/// </remarks>
public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Initializes a new instance of <see cref="UnitOfWorkBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="uow">Unit of work responsible for persisting changes.</param>
    public UnitOfWorkBehavior(IUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Invokes the next handler and commits changes once completed.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">Delegate to invoke the next pipeline stage.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The response from the downstream handler.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next();
        await _uow.SaveChangesAsync(ct); // Commit once per request for transactional integrity.
        return response;
    }
}
