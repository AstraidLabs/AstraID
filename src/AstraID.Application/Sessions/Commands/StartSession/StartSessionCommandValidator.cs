using FluentValidation;

namespace AstraID.Application.Sessions.Commands.StartSession;

public sealed class StartSessionCommandValidator : AbstractValidator<StartSessionCommand>
{
    public StartSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(128);
        RuleFor(x => x.IpAddress).MaximumLength(64);
        RuleFor(x => x.UserAgent).MaximumLength(512);
    }
}
