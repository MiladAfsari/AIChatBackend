using Application.Query.ViewModels;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate.Exceptions;
using MediatR;

namespace Application.Query.UserQueries
{
    public class GetUserByIdQuery : IRequest<UserViewModel>
    {
        public string Id { get; }

        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
    }
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserViewModel>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserViewModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id);

            if (user == null) throw new UserNotFoundException(request.Id);

            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
            };

        }
    }
}
