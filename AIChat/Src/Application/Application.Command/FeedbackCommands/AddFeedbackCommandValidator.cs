using FluentValidation;

namespace Application.Command.FeedbackCommands
{
    public class AddFeedbackCommandValidator : AbstractValidator<AddFeedbackCommand>
    {
        public AddFeedbackCommandValidator()
        {
            RuleFor(x => x.ChatMessageId)
                .NotEmpty()
                .WithMessage("ChatMessageId is required.");

            RuleFor(x => x.Rating)
                .InclusiveBetween((short)0, (short)10)
                .WithMessage("Rating must be between 0 and 10.");
        }
    }
}
