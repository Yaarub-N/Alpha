using Business.Models;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResult> AddUserToRoleAsync(string userId, string roleName);
        Task<UserResult> CreatUserAsync(SignUpFormData form, string roleName = "User");
        Task<UserResult> GetAllUsersAsync(string id);

    }
}