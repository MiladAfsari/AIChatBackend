using Application.Query.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate.Exceptions;
using MediatR;

namespace Application.Query.UserQueries
{
    public class GetUserByTokenQuery : IRequest<UserViewModel>
    {
    }

    public class GetUserByTokenQueryHandler : IRequestHandler<GetUserByTokenQuery, UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public GetUserByTokenQueryHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<UserViewModel> Handle(GetUserByTokenQuery request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenFromRequestAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new UserNotFoundException("Token is invalid or missing.");
            }

            var userId = await _tokenService.GetUserIdFromTokenAsync(token);
            if (userId == null)
            {
                throw new UserNotFoundException("User not found for the provided token.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

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
