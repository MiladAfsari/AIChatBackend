using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

public class AddChatMessageCommand : IRequest<CommandResult>
{
    public Guid? ChatSessionId { get; private set; }
    public string Question { get; private set; }
    public string SessionName { get; private set; }
    public string? Description { get; private set; }

    public AddChatMessageCommand(Guid? chatSessionId, string question, string sessionName, string? description = null)
    {
        ChatSessionId = chatSessionId;
        Question = question;
        SessionName = sessionName;
        Description = description;
    }
}

public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand, CommandResult>
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IApplicationDbContextUnitOfWork _unitOfWork;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IExternalChatBotService _externalChatService;
    private readonly ITokenService _tokenService;

    public AddChatMessageCommandHandler(
        IChatMessageRepository chatMessageRepository,
        IApplicationDbContextUnitOfWork unitOfWork,
        IChatSessionRepository chatSessionRepository,
        IExternalChatBotService externalChatService,
        ITokenService tokenService)
    {
        _chatMessageRepository = chatMessageRepository;
        _unitOfWork = unitOfWork;
        _chatSessionRepository = chatSessionRepository;
        _externalChatService = externalChatService;
        _tokenService = tokenService;
    }

    public async Task<CommandResult> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
    {
        Guid sessionId;
        ChatSession chatSession = null;

        // Validate question
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return CommandResult.Failure("Question must not be null or empty.");
        }

        // Get token and userId
        var token = await _tokenService.GetTokenFromRequestAsync();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }
        var userId = await _tokenService.GetUserIdFromTokenAsync(token);
        if (userId == null)
        {
            return CommandResult.Failure("Unauthorized access to chat session.");
        }

        // 1. Get chat bot response first
        string answer = string.Empty;
        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(40));
            answer = await _externalChatService.GetChatResponseAsync(request.Question).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            return CommandResult.Failure("Request to chat bot service timed out.");
        }
        catch (Exception ex)
        {
            return CommandResult.Failure($"An error occurred while getting response from chat bot: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(answer))
        {
            // Do not create session or message if no answer
            return CommandResult.Failure("Failed to get response from chat bot.");
        }

        // 2. If sessionId provided, check existence and ownership
        if (request.ChatSessionId.HasValue && request.ChatSessionId.Value != Guid.Empty)
        {
            chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId.Value);
            if (chatSession == null)
            {
                return CommandResult.Failure("Chat session not found.");
            }
            if (chatSession.ApplicationUserId != userId.Value)
            {
                return CommandResult.Failure("Unauthorized access to chat session.");
            }
            sessionId = chatSession.Id;
        }
        else
        {
            // 3. If not provided, create new session
            var newSession = new ChatSession
            {
                Id = Guid.CreateVersion7(),
                SessionName = request.SessionName ?? "New Session",
                Description = request.Description,
                ApplicationUserId = userId.Value,
                CreatedAt = DateTime.UtcNow
            };
            await _chatSessionRepository.AddAsync(newSession);
            sessionId = newSession.Id;
            chatSession = newSession;
        }

        // 4. Add chat message
        var chatMessage = new ChatMessage
        {
            ChatSessionId = sessionId,
            Question = request.Question,
            Answer = answer
        };

        await _chatMessageRepository.AddAsync(chatMessage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultData = new { Answer = answer, ChatMessageId = chatMessage.Id, ChatSessionId = sessionId };
        return CommandResult.Success(System.Text.Json.JsonSerializer.Serialize(resultData));
    }
}

