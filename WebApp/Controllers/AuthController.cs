using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models.AuthModels.RegisterModels;

namespace WebApp.Controllers
{
    public class AuthController(
        IAuthService authService,
        INotificationService notificationService,
        IUserService userService,
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager
    ) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IUserService _userService = userService;
        private readonly SignInManager<UserEntity> _signInManager = signInManager;
        private readonly UserManager<UserEntity> _userManager = userManager;

        private static readonly string[] genericErrorMessage = ["Email or password is incorrect."];

        [HttpGet("auth/signup")]
        public IActionResult SignUp(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("auth/signup")]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }

            var formData = model.MapTo<SignUpFormData>();
            var result = await _authService.SignUpAsync(formData);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                return Json(new { redirectUrl = Url.Content("~/admin/overview") });
            }
            //chat gpt 4o
            return BadRequest(new
            {
                errors = new Dictionary<string, string[]> {
                    { "Auth", new[] { result.ErrorMessage ?? "Something went wrong." } }
                }
            });
        }

        [HttpGet("auth/login")]
        public IActionResult Login(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("auth/login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }

            var formData = model.MapTo<SignInFormData>();
            var result = await _authService.SignInAsync(formData);

            if (result.Succeeded)
            {
                return Json(new { redirectUrl = Url.Content("~/admin/overview") });
            }

            return BadRequest(new
            {
                errors = new Dictionary<string, string[]> {
                    { "Auth", genericErrorMessage }
                }
            });
        }

        [HttpPost("auth/externalsignin")]
        public IActionResult ExternalSignIn(string provider, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (string.IsNullOrEmpty(provider))
            {
                ModelState.AddModelError("provider", "Invalid Provider.");
                return RedirectToAction("Login");
            }

            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl })!;
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("auth/externallogincallback")]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            
            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"Remote error: {remoteError}");
                return View("Login");
            }


            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Email not found from provider.");
                return View("Login");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
               
                var logins = await _userManager.GetLoginsAsync(existingUser);
                if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                {
                    await _userManager.AddLoginAsync(existingUser, info);
                }

                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

            var newUser = new UserEntity
            {
                Email = email,
                UserName = email,
                FirstName = firstName ?? "",
                LastName = lastName ?? ""
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("Login");
            }

            await _userManager.AddToRoleAsync(newUser, "User");
            await _userManager.AddLoginAsync(newUser, info);
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            await _notificationService.AddNotificationAsync(new NotificationFormData
            {
                NotificationTypeId = 1,
                NotificationTargetId = 1,
                Message = $"{newUser.FirstName} {newUser.LastName} signed in."
            });

            return LocalRedirect(returnUrl);
        }

        [HttpGet("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return LocalRedirect("~/");
        }
    }
}
