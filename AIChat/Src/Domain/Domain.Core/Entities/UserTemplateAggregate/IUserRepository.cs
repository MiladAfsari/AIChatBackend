namespace Domain.Core.Entities.UserTemplateAggregate
{
    public interface IUserRepository
    {
        Task<bool> AddUsersFromExcelAsync(IEnumerable<(string UserName, string Password, string Role)> usersData);
        Task<bool> AddUserWithRoleAsync(ApplicationUser user, string password, string role);
        Task<ApplicationUser> AuthenticateAsync(string userName, string password);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserByUserNameAsync(string userName);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ChangePasswordAsync(string userName, string newPassword); 
    }
}
