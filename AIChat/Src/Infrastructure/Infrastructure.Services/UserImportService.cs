using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class UserImportService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly ILogger<UserImportService> _logger;

        public UserImportService(IUserRepository userRepository, IApplicationDbContextUnitOfWork unitOfWork, ILogger<UserImportService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ImportUsersFromExcelAsync(string filePath)
        {
            try
            {
                var result = await _userRepository.AddUsersFromExcelAsync(filePath);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
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
