using Application.Service.Common;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.ChatSessionCommands
{
    public class AddChatSessionCommand : IRequest<Guid?>
    {
        public string SessionName { get; private set; }
        public string Description { get; private set; }

        public AddChatSessionCommand(string sessionName, string description)
        {
            SessionName = sessionName;
            Description = description;
        }
    }

    public class AddChatSessionCommandHandler : IRequestHandler<AddChatSessionCommand, Guid?>
    {
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AddChatSessionCommandHandler(IChatSessionRepository chatSessionRepository, IApplicationDbContextUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _chatSessionRepository = chatSessionRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Guid?> Handle(AddChatSessionCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid user.");
            }

            var existingSessions = await _chatSessionRepository.GetByUserIdAsync(userId.Value);
            if (existingSessions.Any(s => s.SessionName == request.SessionName))
            {
                return Guid.Empty;
            }

            var chatSession = new ChatSession
            {
                SessionName = request.SessionName,
                Description = request.Description,
                ApplicationUserId = userId.Value
            };

            await _chatSessionRepository.AddAsync(chatSession);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return chatSession.Id;
        }
    }
}
