using AstraID.Domain.Abstractions;
using MediatR;

namespace AstraID.Application.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _uow;
    public UnitOfWorkBehavior(IUnitOfWork uow) => _uow = uow;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next();
        await _uow.SaveChangesAsync(ct);
        return response;
    }
}
