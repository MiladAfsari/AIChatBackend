using Application.Query.ViewModels;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate.Exceptions;
using MediatR;

namespace Application.Query.UserQueries
{
    public class GetUserByUserNameQuery : IRequest<UserViewModel>
    {
        public string UserName { get; }

        public GetUserByUserNameQuery(string userName)
        {
            UserName = userName;
        }
    }

    public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, UserViewModel>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByUserNameQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserViewModel> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUserNameAsync(request.UserName);

            if (user == null) throw new UserNotFoundException(request.UserName);

            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                DepartmentId = user.DepartmentId,
            };
        }
    }
}
