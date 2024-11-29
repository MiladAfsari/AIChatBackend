using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.UserCommands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        public CreateUserCommand(string userName, string password, string role)
        {
            UserName = userName;
            Password = password;
            Role = role;
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IApplicationDbContextUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                //Email = $"{request.UserName}@example.com",
            };

            var result = await _userRepository.AddUserWithRoleAsync(user, request.Password, request.Role);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
