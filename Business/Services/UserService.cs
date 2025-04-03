

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.IRepositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Business.Services;

public class UserService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, UserManager<UserEntity> userManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<UserResult> GetAllUsersAsync(string id)
    {
        var result = await _userRepository.GetAllAsync();
        return result.MapTo<UserResult>();
    }

    public async Task<UserResult> AddUserToRoleAsync(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Role does't exists" };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "User does't exists" };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "Failed to add user to role" };
        }
        return new UserResult { Succeeded = true, StatusCode = 200 };


    }
    public async Task <UserResult> CreatUserAsync(SignUpFormData form, string roleName="User")
    {
      if(form == null)
        {
            return new UserResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Form data can't be null." };
        }
        var exists = await _userRepository.ExistAsync(x => x.Email == form.Email);
      if (exists.Succeeded)
        {
            return new UserResult { Succeeded = false, StatusCode = 409, ErrorMessage = "User already exists." };
        }

        try
        {

            var userEntity = form.MapTo<UserEntity>();
            userEntity.UserName = userEntity.Email;
            var result = await _userManager.CreateAsync(userEntity, form.Password);
            if (result.Succeeded)
            {
               
                    var roleResult = await AddUserToRoleAsync(userEntity.Id, roleName);
                    if (!roleResult.Succeeded)
                    {
                        return new UserResult { Succeeded = false, StatusCode = 201, ErrorMessage = "Failed to add user to role" };
                    }
                
                return new UserResult { Succeeded = true, StatusCode = 201 };
            }
        }
        catch (Exception ex)
        {

            return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = ex.Message };
        }
        return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "Failed to create user" };
    }

}
