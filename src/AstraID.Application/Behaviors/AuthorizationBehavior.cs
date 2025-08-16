using AstraID.Application.Abstractions;
using MediatR;

namespace AstraID.Application.Behaviors;

public interface IAuthorizedRequest
{
    string[] RequiredScopes { get; }
    string[] RequiredRoles { get; }
}

public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthorizationService _authz;
    public AuthorizationBehavior(IAuthorizationService authz) => _authz = authz;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is IAuthorizedRequest ar)
        {
            var authorized = await _authz.AuthorizeAsync(ar.RequiredScopes, ar.RequiredRoles, ct);
            if (!authorized)
                throw new UnauthorizedAccessException("Forbidden: missing required scopes or roles.");
        }
        return await next();
    }
}
