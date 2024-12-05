using Application.Command.UserCommands;
using FluentValidation;

namespace Application.Command.Validators
{
    public class ChangePasswordCommondValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommondValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.")
                .MinimumLength(6).WithMessage("Old password must be at least 6 characters long.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
                .NotEqual(x => x.OldPassword).WithMessage("New password must be different from the old password.");
        }
    }
}
