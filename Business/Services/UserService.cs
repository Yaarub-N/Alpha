using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class UserService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, UserManager<UserEntity> userManager) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<UserEntity> _userManager = userManager;

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

            // Kontrollera om användaren redan finns med samma e-postadress
            var existingUserByEmail = await _userManager.FindByEmailAsync(form.Email);
            if (existingUserByEmail != null)
            {
                return new UserResult { Succeeded = false, StatusCode = 409, ErrorMessage = "User with this email already exists." };
            }

            try
            {
                var userEntity = form.MapTo<UserEntity>();

                // Se till att användarnamnet är e-postadressen och normalize email
                userEntity.UserName = form.Email; // Här används e-post som användarnamn
                userEntity.NormalizedEmail = form.Email.ToUpper(); // Normalized version of the email
                userEntity.NormalizedUserName = form.Email.ToUpper(); // Det är viktigt att sätta samma normaliserade värde här

                // Skapa användaren
                var createResult = await _userManager.CreateAsync(userEntity, form.Password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = errors };
                }

                // Kontrollera om rollen finns
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    return new UserResult { Succeeded = true, StatusCode = 404, ErrorMessage = $"Role '{roleName}' does not exist." };
                }

                // Lägg till användaren i rollen
                var roleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                if (!roleResult.Succeeded)
                {
                    return new UserResult { Succeeded = false, StatusCode = 500, ErrorMessage = "User created but failed to add role." };
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
}