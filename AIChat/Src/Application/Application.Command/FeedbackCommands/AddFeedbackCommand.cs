using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using System;

namespace Application.Command.FeedbackCommands
{
    public class AddFeedbackCommand : IRequest<CommandResult>
    {
        public Guid ChatMessageId { get; private set; }
        public short Rating { get; private set; }

        public AddFeedbackCommand(Guid chatMessageId, short rating)
        {
            ChatMessageId = chatMessageId;
            Rating = rating;
        }
    }

    public class AddFeedbackCommandHandler : IRequestHandler<AddFeedbackCommand, CommandResult>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AddFeedbackCommandHandler(
            IFeedbackRepository feedbackRepository,
            IApplicationDbContextUnitOfWork unitOfWork,
            ITokenService tokenService,
            IChatMessageRepository chatMessageRepository)
        {
            _feedbackRepository = feedbackRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task<CommandResult> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                return CommandResult.Failure("Invalid token.");
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                return CommandResult.Failure("User not found.");
            }

            var chatMessage = await _chatMessageRepository.GetByIdAsync(request.ChatMessageId);
            if (chatMessage == null)
            {
                return CommandResult.Failure("Chat message not found.");
            }

            var existingFeedbacks = await _feedbackRepository.GetByMessageIdAsync(request.ChatMessageId);
            var feedback = existingFeedbacks.FirstOrDefault(f => f.ApplicationUserId == userId.Value);

            string status;
            if (feedback != null)
            {
                feedback.Rating = request.Rating;
                await _feedbackRepository.UpdateAsync(feedback);
                status = "update";
            }
            else
            {
                feedback = new Feedback
                {
                    Id = Guid.CreateVersion7(),
                    ChatMessageId = request.ChatMessageId,
                    ApplicationUserId = userId.Value,
                    Rating = request.Rating
                };
                await _feedbackRepository.AddAsync(feedback);
                status = "insert";
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultJson = $"{{\"status\":\"{status}\",\"feedbackId\":\"{feedback.Id}\"}}";
            return CommandResult.Success(resultJson);
        }
    }
}
