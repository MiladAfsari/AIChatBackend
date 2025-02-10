using Application.Command.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.UserCommands
{
    public class LogOutCommand : IRequest<LogOutViewModel>
    {
    }

    public class LogOutCommandHandler : IRequestHandler<LogOutCommand, LogOutViewModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LogOutCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LogOutViewModel> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                return new LogOutViewModel
                {
                    Success = false,
                    ErrorMessage = "Token not found."
                };
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                return new LogOutViewModel
                {
                    Success = false,
                    ErrorMessage = "Invalid token."
                };
            }

            var user = await _userRepository.GetUserByIdAsync(userId.ToString());
            if (user == null)
            {
                return new LogOutViewModel
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            await _tokenService.InvalidateTokenAsync(token, userId.Value);

            return new LogOutViewModel
            {
                Success = true
            };
        }
    }
}
