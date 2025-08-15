using FluentValidation;

namespace AstraID.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.DisplayName).MaximumLength(128);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
