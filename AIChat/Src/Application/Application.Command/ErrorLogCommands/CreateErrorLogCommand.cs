using Domain.Core.Error;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.ErrorLogCommands
{
    public class CreateErrorLogCommand : IRequest<Unit>
    {
        public string? Page { get; }
        public string? Block { get; }
        public string? Description { get; }
        public DateTime DateTime { get; }
        public string Username { get; }

        public CreateErrorLogCommand(string? page, string? block, string? description, DateTime dateTime, string username)
        {
            Page = page;
            Block = block;
            Description = description;
            DateTime = dateTime;
            Username = username;
        }
    }

    public class CreateErrorLogCommandHandler : IRequestHandler<CreateErrorLogCommand, Unit>
    {
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly IErrorLogRepository _errorLogRepository;

        public CreateErrorLogCommandHandler(
            IApplicationDbContextUnitOfWork unitOfWork,
            IErrorLogRepository errorLogRepository)
        {
            _unitOfWork = unitOfWork;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<Unit> Handle(CreateErrorLogCommand request, CancellationToken cancellationToken)
        {
            var errorLog = new ErrorLog()
            {
                Page = request.Page,
                Block = request.Block,
                Description = request.Description,
                DateTime = request.DateTime,
                UserName = request.Username
            };
            await _errorLogRepository.CreateErrorLog(errorLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
