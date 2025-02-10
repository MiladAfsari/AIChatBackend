using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.UserCommands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string OldPassword { get; private set; }
        public string NewPassword { get; private set; }

        public ChangePasswordCommand(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IApplicationDbContextUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                return false;
            }

            var user = await _userRepository.GetUserByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            if (_userRepository.AuthenticateAsync(user.UserName, request.OldPassword) == null)
            {
                return false;
            }

            if (!await _userRepository.ChangePasswordAsync(user.UserName, request.NewPassword))
            {
                return false;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
