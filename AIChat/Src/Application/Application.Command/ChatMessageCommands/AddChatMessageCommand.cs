using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

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
        private readonly IChatSessionRepository _chatSessionRepository;

        public AddChatMessageCommandHandler(IChatMessageRepository chatMessageRepository, IApplicationDbContextUnitOfWork unitOfWork, IChatSessionRepository chatSessionRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _unitOfWork = unitOfWork;
            _chatSessionRepository = chatSessionRepository;
        }

        public async Task<Guid?> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
        {
            // Check if the ChatSession exists
            var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId);
            if (chatSession == null)
            {
                return null;
            }

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
    }
}
