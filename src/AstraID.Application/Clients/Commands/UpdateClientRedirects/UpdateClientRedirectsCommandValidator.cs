using FluentValidation;

namespace AstraID.Application.Clients.Commands.UpdateClientRedirects;

public sealed class UpdateClientRedirectsCommandValidator : AbstractValidator<UpdateClientRedirectsCommand>
{
    public UpdateClientRedirectsCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.RedirectUris).NotEmpty();
        RuleForEach(x => x.RedirectUris).Must(BeValidUri).WithMessage("Redirect URI must be absolute HTTP/HTTPS URI");
        RuleForEach(x => x.PostLogoutRedirectUris!).Must(BeValidUri)
            .When(x => x.PostLogoutRedirectUris is not null)
            .WithMessage("Post logout redirect URI must be absolute HTTP/HTTPS URI");
    }

    private static bool BeValidUri(string uri)
        => Uri.TryCreate(uri, UriKind.Absolute, out var parsed) &&
           (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps);
}
