using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class UserService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, UserManager<UserEntity> userManager) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<UserEntity> _userManager = userManager;

        //chat Gpt 4o
        private static readonly SemaphoreSlim _signupLock = new(1, 1);

        public async Task<UserResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            var repositoryResult = await _userRepository.GetAllAsync(orderByDescending: false, sortBy: x => x.UserName!);
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
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Role does not exist" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserResult { Succeeded = false, StatusCode = 404, ErrorMessage = "User does not exist" };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(roleName))
            {
                return new UserResult { Succeeded = true, StatusCode = 200 };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "Failed to assign role" };
            }

            return new UserResult { Succeeded = true, StatusCode = 200 };
        }
        public async Task<UserResult> CreateUserAsync(SignUpFormData form, string roleName = "User")
        {
            if (form == null)
                return new UserResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Form data can't be null." };
            //chat gpt 4o
            await _signupLock.WaitAsync(); 

            try
            {
               
                var normalizedEmail = _userManager.NormalizeEmail(form.Email);
                var normalizedUsername = _userManager.NormalizeName(form.Email); 

                // Kontrollera om användaren redan finns i databasen
                var userExists = await _userManager.Users
                    .AnyAsync(u =>
                        u.NormalizedEmail == normalizedEmail ||
                        u.NormalizedUserName == normalizedUsername);

                if (userExists)
                {
                    return new UserResult
                    {
                        Succeeded = false,
                        StatusCode = 409,
                        ErrorMessage = "User with this email or username already exists."
                    };
                }
                var userEntity = form.MapTo<UserEntity>();
                userEntity.UserName = form.Email;
                userEntity.Email = form.Email;

                var createResult = await _userManager.CreateAsync(userEntity, form.Password);
                if (!createResult.Succeeded)
                {
                    var errorMsg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = errorMsg };
                }
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    return new UserResult { Succeeded = true, StatusCode = 404, ErrorMessage = $"Role '{roleName}' does not exist." };
                }

                var roleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                if (!roleResult.Succeeded)
                {
                    return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "User created but failed to assign role." };
                }

                return new UserResult { Succeeded = true, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new UserResult
                {
                    Succeeded = false,
                    StatusCode = 500,
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message
                };
            }
            finally
            {
                //chat Gpt4o
                _signupLock.Release(); 
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
}