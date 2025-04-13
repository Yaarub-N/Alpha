

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Business.Services;

public class UserService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, UserManager<UserEntity> userManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<UserEntity> _userManager = userManager;


    public async Task<UserResult<IEnumerable<User>>> GetAllUsersAsync()
    {
        var repositoryResult = await _userRepository.GetAllAsync
            (
                orderByDescending: false,
                sortBy: x => x.UserName!
            );

        var entities = repositoryResult.Result;
        var users = entities?.Select(entity => entity.MapTo<User>()) ?? [];

        return new UserResult<IEnumerable<User>> { Succeeded = true, StatusCode = 200, Result = users };
    }




    public async Task<UserResult<User>> GetUserByIdAsync(string id)
    {
        var repositoryResult = await _userRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new UserResult<User> { Succeeded = false, StatusCode = 404, ErrorMessage = $"User with id '{id}' was not found." };

        var user = entity.MapTo<User>();
        return new UserResult<User> { Succeeded = true, StatusCode = 200, Result = user };
    }


    public async Task<UserResult> AddUserToRoleAsync(string userId, string roleName)
    {


        var exist = await _roleManager.RoleExistsAsync(roleName.ToUpper());
        if (!exist)
        {
            return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Role does't exists" };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "User does't exists" };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName.ToUpper());

        if (!result.Succeeded)
        {
            return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "Failed to add user to role" };
        }
        return new UserResult { Succeeded = true, StatusCode = 200 };


    }


public async Task<UserResult> CreatUserAsync(SignUpFormData form, string roleName = "User")
{
    if (form == null)
        return new UserResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Form data can't be null." };

    var exists = await _userRepository.ExistAsync(x => x.Email == form.Email);
    if (exists.Succeeded)
        return new UserResult { Succeeded = false, StatusCode = 409, ErrorMessage = "User already exists." };

    try
    {
        var userEntity = form.MapTo<UserEntity>();
        userEntity.UserName = userEntity.Email;

        var createResult = await _userManager.CreateAsync(userEntity, form.Password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = errors };
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = $"Role '{roleName}' does not exist." };
        }

        var roleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
        if (!roleResult.Succeeded)
        {
            return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = $"User created but failed to add role" };
        }

        return new UserResult { Succeeded = true, StatusCode = 201 };
    }
    catch (Exception ex)
    {
        return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = ex.Message };
    }
}


    public async Task<string> GetDisplayName(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "";

        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }

}
