using Application.Command.ChatMessageCommands;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using Infrastructure.Data.Repository.EfCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.FeedbackCommands
{
    public class AddFeedbackCommand : IRequest<bool>
    {
        public Guid ChatMessageId { get; private set; }
        public Guid ApplicationUserId { get; private set; }
        public bool IsLiked { get; private set; }

        public AddFeedbackCommand(Guid chatMessageId, Guid applicationUserId, bool isLiked)
        {
            ChatMessageId = chatMessageId;
            ApplicationUserId = applicationUserId;
            IsLiked = isLiked;
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
                    IsLiked = request.IsLiked
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
