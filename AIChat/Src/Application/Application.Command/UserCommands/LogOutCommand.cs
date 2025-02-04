using Application.Command.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.UserCommands
{
    public class LogOutCommand : IRequest<LogOutViewModel>
    {
        public Guid UserId { get; }

        public LogOutCommand(Guid userId)
        {
            UserId = userId != Guid.Empty ? userId : throw new ArgumentNullException(nameof(userId));
        }
    }

    public class LogOutCommandHandler : IRequestHandler<LogOutCommand, LogOutViewModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LogOutCommandHandler> _logger;

        public LogOutCommandHandler(IUserRepository userRepository, ITokenService tokenService, ILogger<LogOutCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LogOutViewModel> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Logout attempt for non-existent user: {UserId}", request.UserId);
                return new LogOutViewModel
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            var token = _tokenService.GetTokenFromRequest();
            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.InvalidateToken(token, request.UserId);
            }

            return new LogOutViewModel
            {
                Success = true
            };
        }
    }
}
