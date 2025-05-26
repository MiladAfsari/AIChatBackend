using Application.Service.Common;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.ChatSessionCommands
{
    public class UpdateChatSessionNameBySessionIdCommand : IRequest<CommandResult>
    {
        public Guid ChatSessionId { get; private set; }
        public string NewSessionName { get; private set; }

        public UpdateChatSessionNameBySessionIdCommand(Guid chatSessionId, string newSessionName)
        {
            ChatSessionId = chatSessionId;
            NewSessionName = newSessionName;
        }
    }

    public class UpdateChatSessionNameBySessionIdCommandHandler : IRequestHandler<UpdateChatSessionNameBySessionIdCommand, CommandResult>
    {
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public UpdateChatSessionNameBySessionIdCommandHandler(
            IChatSessionRepository chatSessionRepository,
            IApplicationDbContextUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _chatSessionRepository = chatSessionRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<CommandResult> Handle(UpdateChatSessionNameBySessionIdCommand request, CancellationToken cancellationToken)
        {
            var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId);
            if (chatSession == null)
            {
                return CommandResult.Failure("Chat session not found.");
            }

            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null || chatSession.ApplicationUserId != userId.Value)
            {
                return CommandResult.Failure("Unauthorized access to chat session.");
            }

            chatSession.SessionName = request.NewSessionName;
            await _chatSessionRepository.UpdateAsync(chatSession);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultData = new { ChatSessionId = chatSession.Id, NewSessionName = chatSession.SessionName };
            return CommandResult.Success(System.Text.Json.JsonSerializer.Serialize(resultData));
        }
    }
}
