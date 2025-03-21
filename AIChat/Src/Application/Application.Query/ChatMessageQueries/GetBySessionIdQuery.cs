using Application.Query.ViewModels.Application.Query.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using MediatR;

namespace Application.Query.ChatMessageQueries
{
    public class GetBySessionIdQuery : IRequest<PaginatedResult<GetChatMessagesBySessionIdViewModel>>
    {
        public Guid SessionId { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }

        public GetBySessionIdQuery(Guid sessionId, int pageNumber, int pageSize)
        {
            SessionId = sessionId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetBySessionIdQueryHandler : IRequestHandler<GetBySessionIdQuery, PaginatedResult<GetChatMessagesBySessionIdViewModel>>
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly ITokenService _tokenService;

        public GetBySessionIdQueryHandler(IChatMessageRepository chatMessageRepository, IChatSessionRepository chatSessionRepository, ITokenService tokenService)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatSessionRepository = chatSessionRepository;
            _tokenService = tokenService;
        }

        public async Task<PaginatedResult<GetChatMessagesBySessionIdViewModel>> Handle(GetBySessionIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the ChatSession exists
            var chatSession = await _chatSessionRepository.GetByIdAsync(request.SessionId);
            if (chatSession == null)
            {
                return new PaginatedResult<GetChatMessagesBySessionIdViewModel>
                {
                    Items = new List<GetChatMessagesBySessionIdViewModel>(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0
                };
            }

            // Check if the ChatSession belongs to the current user
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null || chatSession.ApplicationUserId != userId.Value)
            {
                return new PaginatedResult<GetChatMessagesBySessionIdViewModel>
                {
                    Items = new List<GetChatMessagesBySessionIdViewModel>(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0
                };
            }

            var chatMessages = await _chatMessageRepository.GetChatsWithFeedbackBySessionIdAsync(request.SessionId);

            var paginatedMessages = chatMessages
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(cm => new GetChatMessagesBySessionIdViewModel
                {
                    ChatSessionId = cm.ChatSessionId,
                    ChatMessageId = cm.Id,
                    Question = cm.Question,
                    Answer = cm.Answer,
                    CreatedAt = cm.CreatedAt,
                    Feedback = cm.Feedback != null ? new FeedbackViewModel
                    {
                        Rating = cm.Feedback.Rating
                    } : null
                });

            return new PaginatedResult<GetChatMessagesBySessionIdViewModel>
            {
                Items = paginatedMessages.ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = chatMessages.Count()
            };
        }
    }
}
