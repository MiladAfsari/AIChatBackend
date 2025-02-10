using Application.Query.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using MediatR;

namespace Application.Query.ChatSessionQueries
{
    public class GetAllChatSessionsQueryByUserId : IRequest<IEnumerable<GetChatSessionsByUserIdViewModel>>
    {
        public GetAllChatSessionsQueryByUserId() { }
    }

    public class GetByUserIdQueryHandler : IRequestHandler<GetAllChatSessionsQueryByUserId, IEnumerable<GetChatSessionsByUserIdViewModel>>
    {
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly ITokenService _tokenService;

        public GetByUserIdQueryHandler(IChatSessionRepository chatSessionRepository, ITokenService tokenService)
        {
            _chatSessionRepository = chatSessionRepository;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<GetChatSessionsByUserIdViewModel>> Handle(GetAllChatSessionsQueryByUserId request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                return Enumerable.Empty<GetChatSessionsByUserIdViewModel>();
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                return Enumerable.Empty<GetChatSessionsByUserIdViewModel>();
            }

            var chatSessions = await _chatSessionRepository.GetByUserIdAsync(userId.Value);
            return chatSessions.Select(cs => new GetChatSessionsByUserIdViewModel
            {
                Id = cs.Id,
                SessionName = cs.SessionName,
                Description = cs.Description
            });
        }
    }
}
