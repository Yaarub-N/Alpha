using Business.Models;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResult> AddUserToRoleAsync(string userId, string roleName);
        Task<UserResult> CreatUserAsync(SignUpFormData form, string roleName = "User");
        Task<UserResult<IEnumerable<User>>> GetAllUsersAsync();
        Task<string> GetDisplayName(string userId);
        Task<UserResult<User>> GetUserByIdAsync(string id);
    }
}