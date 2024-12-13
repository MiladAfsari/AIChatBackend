using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.ChatMessageCommands
{
    public class AddChatMessageCommand : IRequest<Guid?>
    {
        public Guid ChatSessionId { get; private set; }
        public string Question { get; private set; }
        public string Answer { get; private set; }

        public AddChatMessageCommand(Guid chatSessionId, string question, string answer)
        {
            ChatSessionId = chatSessionId;
            Question = question;
            Answer = answer;
        }
    }

    public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand, Guid?>
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<AddChatMessageCommandHandler> _logger;

        public AddChatMessageCommandHandler(IChatMessageRepository chatMessageRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<AddChatMessageCommandHandler> logger)
        {
            _chatMessageRepository = chatMessageRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid?> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var chatMessage = new ChatMessage
                {
                    ChatSessionId = request.ChatSessionId,
                    Question = request.Question,
                    Answer = request.Answer
                };

                await _chatMessageRepository.AddAsync(chatMessage);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return chatMessage.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding chat message");
                return null;
            }
        }
    }
}
