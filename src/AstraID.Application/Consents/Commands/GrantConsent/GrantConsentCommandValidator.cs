using FluentValidation;

namespace AstraID.Application.Consents.Commands.GrantConsent;

public sealed class GrantConsentCommandValidator : AbstractValidator<GrantConsentCommand>
{
    public GrantConsentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Scopes).NotEmpty();
        RuleForEach(x => x.Scopes).NotEmpty().MaximumLength(200);
    }
}
