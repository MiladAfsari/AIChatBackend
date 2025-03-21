using Application.Service.Common;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

public class DeleteChatSessionWithMessagesCommand : IRequest<CommandResult>
{
    public Guid ChatSessionId { get; private set; }

    public DeleteChatSessionWithMessagesCommand(Guid chatSessionId)
    {
        ChatSessionId = chatSessionId;
    }
}

public class DeleteChatSessionWithMessagesCommandHandler : IRequestHandler<DeleteChatSessionWithMessagesCommand, CommandResult>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IApplicationDbContextUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public DeleteChatSessionWithMessagesCommandHandler(IChatSessionRepository chatSessionRepository, IApplicationDbContextUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _chatSessionRepository = chatSessionRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<CommandResult> Handle(DeleteChatSessionWithMessagesCommand request, CancellationToken cancellationToken)
    {
        // Check if the ChatSession exists
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId);
        if (chatSession == null)
        {
            return CommandResult.Failure("Chat session not found.");
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
            return CommandResult.Failure("Unauthorized access to chat session.");
        }

        await _chatSessionRepository.DeleteChatSessionWithMessagesAsync(request.ChatSessionId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult.Success("Chat session and its messages deleted successfully.");
    }
}
