using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.FeedbackCommands
{
    public class AddFeedbackCommand : IRequest<bool>
    {
        public Guid ChatMessageId { get; private set; }
        public Guid ApplicationUserId { get; private set; }
        public short Rating { get; private set; }

        public AddFeedbackCommand(Guid chatMessageId, Guid applicationUserId, short rating)
        {
            ChatMessageId = chatMessageId;
            ApplicationUserId = applicationUserId;
            Rating = rating;
        }
    }

    public class AddFeedbackCommandHandler : IRequestHandler<AddFeedbackCommand, bool>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<AddFeedbackCommandHandler> _logger;

        public AddFeedbackCommandHandler(IFeedbackRepository feedbackRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<AddFeedbackCommandHandler> logger)
        {
            _feedbackRepository = feedbackRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var feedback = new Feedback
                {
                    Id = Guid.NewGuid(),
                    ChatMessageId = request.ChatMessageId,
                    ApplicationUserId = request.ApplicationUserId,
                    Rating = request.Rating
                };

                await _feedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Feedback added successfully for ChatMessageId: {ChatMessageId}", request.ChatMessageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding feedback for ChatMessageId: {ChatMessageId}", request.ChatMessageId);
                return false;
            }
        }
    }
}
