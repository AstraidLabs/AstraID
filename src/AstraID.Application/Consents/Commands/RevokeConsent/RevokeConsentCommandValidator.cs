using FluentValidation;

namespace AstraID.Application.Consents.Commands.RevokeConsent;

public sealed class RevokeConsentCommandValidator : AbstractValidator<RevokeConsentCommand>
{
    public RevokeConsentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
    }
}
