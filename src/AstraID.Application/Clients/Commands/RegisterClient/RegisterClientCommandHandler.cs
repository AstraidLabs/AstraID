using System.Linq;
using AstraID.Application.Common;
using AstraID.Domain.Services;
using AstraID.Domain.ValueObjects;
using MediatR;

namespace AstraID.Application.Clients.Commands.RegisterClient;

public sealed class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, Result<Guid>>
{
    private readonly ClientDomainService _clients;

    public RegisterClientCommandHandler(ClientDomainService clients) => _clients = clients;

    public async Task<Result<Guid>> Handle(RegisterClientCommand request, CancellationToken ct)
    {
        var scopes = request.Scopes.Select(Scope.Create);
        var redirects = request.RedirectUris.Select(RedirectUri.Create);
        var postLogout = request.PostLogoutRedirectUris?.Select(RedirectUri.Create) ?? Enumerable.Empty<RedirectUri>();
        var result = await _clients.RegisterAsync(request.ClientId, request.DisplayName ?? request.ClientId,
            request.Type, scopes, redirects, postLogout, Enumerable.Empty<string>(), null, null, ct);
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!.Code, result.Error!.Message);
        return Result<Guid>.Success(result.Value!.Id);
    }
}
