using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
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

        public CreateErrorLogCommand(string? page, string? block, string? description, DateTime dateTime)
        {
            Page = page;
            Block = block;
            Description = description;
            DateTime = dateTime;
        }
    }

    public class CreateErrorLogCommandHandler : IRequestHandler<CreateErrorLogCommand, Unit>
    {
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public CreateErrorLogCommandHandler(
            IApplicationDbContextUnitOfWork unitOfWork,
            IErrorLogRepository errorLogRepository,
            ITokenService tokenService,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _errorLogRepository = errorLogRepository;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(CreateErrorLogCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            string? userName = null;

            if (!string.IsNullOrEmpty(token))
            {
                var userId = await _tokenService.GetUserIdFromTokenAsync(token);
                if (userId != null)
                {
                    var user = await _userRepository.GetUserByIdAsync(userId.ToString());
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                }
            }

            var errorLog = new ErrorLog()
            {
                Page = request.Page,
                Block = request.Block,
                Description = request.Description,
                DateTime = request.DateTime,
                UserName = userName
            };
            await _errorLogRepository.CreateErrorLog(errorLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
