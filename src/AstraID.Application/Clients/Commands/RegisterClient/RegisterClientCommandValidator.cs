using FluentValidation;

namespace AstraID.Application.Clients.Commands.RegisterClient;

public sealed class RegisterClientCommandValidator : AbstractValidator<RegisterClientCommand>
{
    public RegisterClientCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DisplayName).MaximumLength(200);
        RuleFor(x => x.Scopes).NotEmpty();
        RuleForEach(x => x.Scopes).NotEmpty().MaximumLength(200);
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
