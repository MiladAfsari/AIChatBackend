using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.FeedbackCommands
{
    public class AddFeedbackCommand : IRequest<bool>
    {
        public Guid ChatMessageId { get; private set; }
        public short Rating { get; private set; }

        public AddFeedbackCommand(Guid chatMessageId, short rating)
        {
            ChatMessageId = chatMessageId;
            Rating = rating;
        }
    }

    public class AddFeedbackCommandHandler : IRequestHandler<AddFeedbackCommand, bool>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AddFeedbackCommandHandler(IFeedbackRepository feedbackRepository, IApplicationDbContextUnitOfWork unitOfWork, ITokenService tokenService, IChatMessageRepository chatMessageRepository)
        {
            _feedbackRepository = feedbackRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task<bool> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                return false;
            }

            var chatMessage = await _chatMessageRepository.GetByIdAsync(request.ChatMessageId);
            if (chatMessage == null)
            {
                return false;
            }

            // Check for duplicate feedback
            var existingFeedback = await _feedbackRepository.GetByMessageIdAsync(request.ChatMessageId);
            if (existingFeedback.Any(f => f.ApplicationUserId == userId.Value))
            {
                return false;
            }

            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                ChatMessageId = request.ChatMessageId,
                ApplicationUserId = userId.Value,
                Rating = request.Rating
            };

            await _feedbackRepository.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
