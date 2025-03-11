using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

public class AddChatMessageCommand : IRequest<CommandResult>
{
    public Guid ChatSessionId { get; private set; }
    public string Question { get; private set; }

    public AddChatMessageCommand(Guid chatSessionId, string question)
    {
        ChatSessionId = chatSessionId;
        Question = question;
    }
}

public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand, CommandResult>
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IApplicationDbContextUnitOfWork _unitOfWork;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IExternalChatBotService _externalChatService;

    public AddChatMessageCommandHandler(IChatMessageRepository chatMessageRepository, IApplicationDbContextUnitOfWork unitOfWork, IChatSessionRepository chatSessionRepository, IExternalChatBotService externalChatService)
    {
        _chatMessageRepository = chatMessageRepository;
        _unitOfWork = unitOfWork;
        _chatSessionRepository = chatSessionRepository;
        _externalChatService = externalChatService;
    }

    public async Task<CommandResult> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
    {
        // Check if the ChatSession exists
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId);
        if (chatSession == null)
        {
            return CommandResult.Failure("Chat session not found.");
        }

        // Get response from external chat bot service with timeout handling
        string answer;
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

        if (string.IsNullOrEmpty(answer) || string.IsNullOrWhiteSpace(answer))
        {
            return CommandResult.Failure("Failed to get response from chat bot.");
        }

        var chatMessage = new ChatMessage
        {
            ChatSessionId = request.ChatSessionId,
            Question = request.Question,
            Answer = answer
        };

        await _chatMessageRepository.AddAsync(chatMessage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(answer);
    }
}

public class CommandResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public string Data { get; }

    private CommandResult(bool isSuccess, string message, string data = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static CommandResult Success(string data) => new CommandResult(true, "Success", data);
    public static CommandResult Failure(string message) => new CommandResult(false, message);
}
