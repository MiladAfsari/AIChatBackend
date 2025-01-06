using Domain.Core.Entities.UserTemplateAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }


        // Bulk import users with roles from Excel data
        public async Task<bool> AddUsersFromExcelAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var usersData = new List<(string UserName, string Password, string Role, string FullName)>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var userName = worksheet.Cells[row, 1].Text;
                        var password = worksheet.Cells[row, 2].Text;
                        var role = string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Text) ? "Staff" : worksheet.Cells[row, 3].Text;
                        var fullName = worksheet.Cells[row, 4].Text;

                        // Validate data
                        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
                        {
                            _logger.LogWarning("Invalid data at row {RowNumber}: {UserName}, {Password}, {Role}", row, userName, password, role);
                            continue;
                        }
                        var existingUser = await _userManager.FindByNameAsync(userName);
                        if (existingUser != null)
                        {
                            _logger.LogWarning("User {UserName} already exists and will not be added again.", userName);
                            continue;
                        }
                        usersData.Add((userName, password, role, fullName));
                    }
                }

                foreach (var userData in usersData)
                {
                    var user = new ApplicationUser
                    {
                        UserName = userData.UserName,
                        FullName = userData.FullName
                    };

                    var result = await AddUserWithRoleAsync(user, userData.Password, userData.Role);
                    if (!result)
                    {
                        _logger.LogError("Failed to add user {UserName} with role {Role}", userData.UserName, userData.Role);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while importing users from Excel file {FilePath}", filePath);
                return false;
            }
        }


        // Get a user by username
        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        // Add a single user with a role
        public async Task<bool> AddUserWithRoleAsync(ApplicationUser user, string password, string role)
        {
            try
            {
                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user {UserName}", user.UserName);
                    return false;
                }

                if (string.IsNullOrEmpty(role))
                {
                    _logger.LogError("Role is null or empty for user {UserName}", user.UserName);
                    await _userManager.DeleteAsync(user); // Rollback user creation if role is null or empty
                    return false;
                }

                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to create role {Role}", role);
                        await _userManager.DeleteAsync(user); // Rollback user creation if role creation fails
                        return false;
                    }
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                {
                    _logger.LogError("Failed to add role {Role} to user {UserName}", role, user.UserName);
                    await _userManager.DeleteAsync(user); // Rollback user creation if role assignment fails
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user {UserName} with role {Role}", user.UserName, role);
                return false;
            }
        }

        // Authenticate user with credentials
        public async Task<ApplicationUser> AuthenticateAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null && await _userManager.CheckPasswordAsync(user, password) ? user : null;
        }

        // Get all users
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Select(user => new ApplicationUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    DepartmentId = user.DepartmentId,
                })
                .ToListAsync();
        }

        // Get a specific user by ID
        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        // Update user details
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null) return false;

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;

            var updateResult = await _userManager.UpdateAsync(existingUser);
            return updateResult.Succeeded;
        }

        // Delete a user by ID
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var deleteResult = await _userManager.DeleteAsync(user);
            return deleteResult.Succeeded;
        }

        // Change user password
        public async Task<bool> ChangePasswordAsync(string userName, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }
    }
}
