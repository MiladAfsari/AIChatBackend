using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Service.Common;
using Domain.Core.Entities.UserTemplateAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService, ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginViewModel> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            // Validate user credentials
            var user = await _userRepository.GetUserByUserNameAsync(request.UserName);
            if (user == null || !VerifyPassword(user, request.Password))
            {
                _logger.LogWarning("Invalid login attempt for user: {UserName}", request.UserName);
                return new LoginViewModel
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);

            return new LoginViewModel
            {
                Success = true,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user: {UserName}", request.UserName);
            return new LoginViewModel
            {
                Success = false,
                ErrorMessage = "An error occurred while processing your request."
            };
        }
    }

    private bool VerifyPassword(ApplicationUser user, string password)
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
