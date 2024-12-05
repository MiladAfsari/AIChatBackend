using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.UserCommands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string UserName { get; private set; }
        public string OldPassword { get; private set; }
        public string NewPassword { get; private set; }

        public ChangePasswordCommand(string userName, string oldPassword, string newPassword)
        {
            UserName = userName;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the old password
                var user = await _userRepository.AuthenticateAsync(request.UserName, request.OldPassword);
                if (user == null)
                {
                    _logger.LogWarning("Invalid old password for user {UserName}", request.UserName);
                    return false;
                }

                // Change the password
                var isPasswordChanged = await _userRepository.ChangePasswordAsync(request.UserName, request.NewPassword);
                if (!isPasswordChanged)
                {
                    _logger.LogError("Failed to change password for user {UserName}", request.UserName);
                    return false;
                }

                // Save changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Password changed successfully for user {UserName}", request.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing password for user {UserName}", request.UserName);
                return false;
            }
        }
    }
}
