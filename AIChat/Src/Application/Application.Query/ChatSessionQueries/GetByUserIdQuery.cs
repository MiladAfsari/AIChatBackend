using Application.Query.ViewModels;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Query.ChatSessionQueries
{
    public class GetByUserIdQuery : IRequest<IEnumerable<GetChatSessionsByUserIdViewModel>>
    {
        public Guid UserId { get; private set; }

        public GetByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetByUserIdQueryHandler : IRequestHandler<GetByUserIdQuery, IEnumerable<GetChatSessionsByUserIdViewModel>>
    {
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly ILogger<GetByUserIdQueryHandler> _logger;

        public GetByUserIdQueryHandler(IChatSessionRepository chatSessionRepository, ILogger<GetByUserIdQueryHandler> logger)
        {
            _chatSessionRepository = chatSessionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GetChatSessionsByUserIdViewModel>> Handle(GetByUserIdQuery request, CancellationToken cancellationToken)
        {
            var chatSessions = await _chatSessionRepository.GetByUserIdAsync(request.UserId);
            return chatSessions.Select(cs => new GetChatSessionsByUserIdViewModel
            {
                Id = cs.Id,
                SessionName = cs.SessionName,
                Description = cs.Description
            });
        }
    }
}
