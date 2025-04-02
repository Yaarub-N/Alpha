

using Business.Models;
using Data.IRepositories;
using Domain.Extentions;

namespace Business.Services;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResult> GetAllUsersAsync(string id)
    {
        var result = await _userRepository.GetAllAsync();
        return result.MapTo<UserResult>();
    }
 
}
