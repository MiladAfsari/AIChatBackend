using Application.Command.ChatMessageCommands;
using FluentValidation;

namespace Application.Command.Validators
{
    public class AddChatMessageCommandValidator : AbstractValidator<AddChatMessageCommand>
    {
        public AddChatMessageCommandValidator()
        {
            RuleFor(x => x.ChatSessionId)
                .NotEmpty().WithMessage("Chat Session Id is required");
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question is required");
        }
    }
}
