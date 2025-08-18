using FluentValidation;

namespace AstraID.Application.Clients.Commands.RotateClientSecret;

public sealed class RotateClientSecretCommandValidator : AbstractValidator<RotateClientSecretCommand>
{
    public RotateClientSecretCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.NewSecret).NotEmpty().MinimumLength(8);
    }
}
