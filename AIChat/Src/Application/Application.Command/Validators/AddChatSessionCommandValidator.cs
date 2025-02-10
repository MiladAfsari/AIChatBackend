using Application.Command.ChatSessionCommands;
using FluentValidation;

namespace Application.Command.Validators
{
    public class AddChatSessionCommandValidator : AbstractValidator<AddChatSessionCommand>
    {
        public AddChatSessionCommandValidator()
        {
            RuleFor(x => x.SessionName)
                .NotEmpty().WithMessage("Session Name is required")
                .MaximumLength(200).WithMessage("Session Name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        }
    }
}
