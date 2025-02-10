using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Command.UserCommands
{
    public class LoginCommand : IRequest<LoginViewModel>
    {
        public string UserName { get; }
        public string Password { get; }

        public LoginCommand(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginViewModel>
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginViewModel> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate user credentials
        var user = await _userRepository.GetUserByUserNameAsync(request.UserName);
        if (user == null || !VerifyPassword(user, request.Password))
        {
            return new LoginViewModel
            {
                Success = false,
                ErrorMessage = "Invalid username or password."
            };
        }

        // Generate token
        var token = await _tokenService.GenerateTokenAsync(user);

        return new LoginViewModel
        {
            Success = true,
            Token = token
        };
    }

    private bool VerifyPassword(ApplicationUser user, string password)
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
