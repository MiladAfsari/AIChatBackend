using Application.Query.ViewModels.Application.Query.ViewModels;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Query.ChatMessageQueries
{
    public class GetBySessionIdQuery : IRequest<IEnumerable<GetChatMessagesBySessionIdViewModel>>
    {
        public Guid SessionId { get; private set; }

        public GetBySessionIdQuery(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }

    public class GetBySessionIdQueryHandler : IRequestHandler<GetBySessionIdQuery, IEnumerable<GetChatMessagesBySessionIdViewModel>>
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly ILogger<GetBySessionIdQueryHandler> _logger;

        public GetBySessionIdQueryHandler(IChatMessageRepository chatMessageRepository, ILogger<GetBySessionIdQueryHandler> logger)
        {
            _chatMessageRepository = chatMessageRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GetChatMessagesBySessionIdViewModel>> Handle(GetBySessionIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var chatMessages = await _chatMessageRepository.GetChatsWithFeedbackBySessionIdAsync(request.SessionId);

                return chatMessages.Select(cm => new GetChatMessagesBySessionIdViewModel
                {
                    ChatSessionId = cm.ChatSessionId,
                    Question = cm.Question,
                    Answer = cm.Answer,
                    Feedback = cm.Feedback != null ? new FeedbackViewModel
                    {
                        ChatMessageId = cm.Feedback.ChatMessageId,
                        ApplicationUserId = cm.Feedback.ApplicationUserId,
                        Rating = cm.Feedback.Rating
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat messages for session {SessionId}", request.SessionId);
                return Enumerable.Empty<GetChatMessagesBySessionIdViewModel>();
            }
        }
    }
}
