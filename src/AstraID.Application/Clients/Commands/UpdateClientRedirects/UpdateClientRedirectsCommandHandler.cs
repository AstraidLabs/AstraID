using System.Linq;
using AstraID.Application.Common;
using AstraID.Domain.Repositories;
using AstraID.Domain.ValueObjects;
using MediatR;

namespace AstraID.Application.Clients.Commands.UpdateClientRedirects;

public sealed class UpdateClientRedirectsCommandHandler : IRequestHandler<UpdateClientRedirectsCommand, Result>
{
    private readonly IClientRepository _clients;

    public UpdateClientRedirectsCommandHandler(IClientRepository clients) => _clients = clients;

    public async Task<Result> Handle(UpdateClientRedirectsCommand request, CancellationToken ct)
    {
        var client = await _clients.GetByClientIdAsync(request.ClientId, null, ct);
        if (client == null)
            return Result.Failure("client.notFound", "Client not found.");

        var newRedirects = request.RedirectUris.Select(RedirectUri.Create).ToList();
        var existing = client.RedirectUris.Select(r => r.Value).ToList();
        foreach (var r in existing.Except(newRedirects.Select(n => n.Value)))
            client.RemoveRedirect(r);
        foreach (var r in newRedirects.Select(n => n.Value).Except(existing))
            client.AddRedirect(RedirectUri.Create(r));

        var newPost = request.PostLogoutRedirectUris?.Select(RedirectUri.Create).ToList() ?? new List<RedirectUri>();
        var existingPost = client.PostLogoutRedirectUris.Select(r => r.Value).ToList();
        foreach (var r in existingPost.Except(newPost.Select(n => n.Value)))
            client.RemovePostLogoutRedirect(r);
        foreach (var r in newPost.Select(n => n.Value).Except(existingPost))
            client.AddPostLogoutRedirect(RedirectUri.Create(r));

        return Result.Success();
    }
}
