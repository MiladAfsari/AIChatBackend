using FluentValidation;

namespace Application.Command.Validators
{
    public class AddChatMessageCommandValidator : AbstractValidator<AddChatMessageCommand>
    {
        public AddChatMessageCommandValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question is required");
        }
    }
}
