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

        public GetBySessionIdQueryHandler(IChatMessageRepository chatMessageRepository)
        {
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task<IEnumerable<GetChatMessagesBySessionIdViewModel>> Handle(GetBySessionIdQuery request, CancellationToken cancellationToken)
        {
            var chatMessages = await _chatMessageRepository.GetChatsWithFeedbackBySessionIdAsync(request.SessionId);

            return chatMessages.Select(cm => new GetChatMessagesBySessionIdViewModel
            {
                ChatSessionId = cm.ChatSessionId,
                ChatMessageId = cm.Id,
                Question = cm.Question,
                Answer = cm.Answer,
                Feedback = cm.Feedback != null ? new FeedbackViewModel
                {
                    Rating = cm.Feedback.Rating
                } : null
            });
        }
    }
}
