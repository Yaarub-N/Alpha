

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Extentions;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;




public class AuthService(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, IUserService userService) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly IUserService _userService = userService;

    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, ErrorMessage = "form data can't be null." };

        var result = await _userService.CreatUserAsync(formData);
        return result.Succeeded ?
            new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = 409, ErrorMessage = "User creation failed." };
    }

    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, ErrorMessage = "form data can't be null." };

        var signInResult = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);
        return signInResult.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 200 }
            : new AuthResult { Succeeded = false, StatusCode = 401 };
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
 