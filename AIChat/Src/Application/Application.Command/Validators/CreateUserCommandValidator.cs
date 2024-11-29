using Application.Command.UserCommands;
using FluentValidation;

namespace Application.Command.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            // Validate UserName
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            // Validate Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\@\#\$\%\^\&\*\(\)\_\+\-]").WithMessage("Password must contain at least one special character.");

            // Validate Role
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(50).WithMessage("Role must not exceed 50 characters.");
        }
    }
}
