using Business.Models;
using Domain.Models;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResult> AddUserToRoleAsync(string userId, string roleName);
        Task<UserResult> CreatUserAsync(SignUpFormData form, string roleName = "User");
        Task<UserResult<IEnumerable<User>>> GetAllUsersAsync();
        Task<UserResult> GetUserByIdAsync(string id);
    }
}