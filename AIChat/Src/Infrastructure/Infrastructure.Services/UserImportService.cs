using Domain.Core.Entities.UserTemplateAggregate;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class UserImportService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserImportService> _logger;

        public UserImportService(IUserRepository userRepository, ILogger<UserImportService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ImportUsersFromExcelAsync(string filePath)
        {
            try
            {
                var result = await _userRepository.AddUsersFromExcelAsync(filePath);
                if (result)
                {
                    _logger.LogInformation("User import from {FilePath} completed successfully.", filePath);
                }
                else
                {
                    _logger.LogError("User import from {FilePath} failed.", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while importing users from {FilePath}", filePath);
            }
        }
    }
}
