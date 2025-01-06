using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Command.UserCommands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        public string FullName { get; private set; }
        public short DepartmentId { get; private set; }

        public CreateUserCommand(string userName, string password, string role, string fullName, short departmentId)
        {
            UserName = userName;
            Password = password;
            Role = role;
            FullName = fullName;
            DepartmentId = departmentId;
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    //Email = $"{request.UserName}@example.com", // Uncomment if needed
                    FullName = request.FullName,
                    DepartmentId = request.DepartmentId
                };

                var result = await _userRepository.AddUserWithRoleAsync(user, request.Password, request.Role);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return false;
            }
        }
    }
}
