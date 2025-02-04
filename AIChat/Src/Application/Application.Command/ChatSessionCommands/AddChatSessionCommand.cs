using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.ChatSessionCommands
{
    public class AddChatSessionCommand : IRequest<Guid>
    {
        public string SessionName { get; private set; }
        public string Description { get; private set; }
        public Guid ApplicationUserId { get; private set; }

        public AddChatSessionCommand(string sessionName, string description, Guid applicationUserId)
        {
            SessionName = sessionName;
            Description = description;
            ApplicationUserId = applicationUserId;
        }
    }

    public class AddChatSessionCommandHandler : IRequestHandler<AddChatSessionCommand, Guid>
    {
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<AddChatSessionCommandHandler> _logger;

        public AddChatSessionCommandHandler(IChatSessionRepository chatSessionRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<AddChatSessionCommandHandler> logger)
        {
            _chatSessionRepository = chatSessionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddChatSessionCommand request, CancellationToken cancellationToken)
        {
            var chatSession = new ChatSession
            {
                SessionName = request.SessionName,
                Description = request.Description,
                ApplicationUserId = request.ApplicationUserId
            };

            var result = await _chatSessionRepository.AddAsync(chatSession);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
