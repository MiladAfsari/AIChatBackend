using Domain.Core.Entities.UserTemplateAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.EfCore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

      
        // Bulk import users with roles from Excel data
        public async Task<bool> AddUsersFromExcelAsync(IEnumerable<(string UserName, string Password, string Role)> usersData)
        {
            foreach (var (userName, password, role) in usersData)
            {
                if (!await AddUserWithRoleAsync(
                        new ApplicationUser
                        {
                            UserName = userName,
                            FullName = userName,
                            Email = $"{userName}@example.com"
                        },
                        password,
                        role))
                {
                    return false; // Stop if any user creation fails
                }
            }
            return true;
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
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded) return false;

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            return roleResult.Succeeded;
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
                    Email = user.Email
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
            var existingUser = await _userManager.FindByIdAsync(user.Id);
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
