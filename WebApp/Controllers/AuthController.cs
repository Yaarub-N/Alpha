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
        private static readonly string[] value = ["Email or password is incorrect."];

        [HttpGet("auth/signup")]
        public IActionResult SignUp(string returnUrl = "~/")
        {

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "";
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

            var signUpFormData = model.MapTo<SignUpFormData>();
            var authResult = await _authService.SignUpAsync(signUpFormData);

            if (authResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                return Json(new { redirectUrl = Url.Content("~/admin/overview") });
            }

            return BadRequest(new
            {
                errors = new Dictionary<string, string[]>
        {
                    {  "Auth", new[] { authResult.ErrorMessage ?? "Something went wrong." } }
        }
            });
        }


        [HttpGet("auth/login")]
        public IActionResult Login(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "";
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

            var signInFormData = model.MapTo<SignInFormData>();
            var authResult = await _authService.SignInAsync(signInFormData);

            if (authResult.Succeeded)
            {
                return Json(new { redirectUrl = Url.Content("~/admin/overview") });
            }

            // chat gpt4o

            return BadRequest(new
            {
                errors = new Dictionary<string, string[]>
        {
            { "Auth", value }
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

   
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"remoteError: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");  //null
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true
            );

            if (signInResult.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var userEntity = await _userManager.FindByEmailAsync(email!);

                if (userEntity != null)
                {
                    await _signInManager.SignInAsync(userEntity, isPersistent: false);
                    var notification = new NotificationFormData
                    {
                        NotificationTypeId = 1,
                        NotificationTargetId = 1,
                        Message = $"{userEntity.FirstName} {userEntity.LastName} signed in.",
                    
                    };
                    await _notificationService.AddNotificationAsync(notification);
                }

                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

                var user = new UserEntity
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName
                };
                //chat Gpt 4o
                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var notification = new NotificationFormData
                    {
                        NotificationTypeId = 1,
                        NotificationTargetId = 1,
                        Message = $"{user.FirstName} {user.LastName} signed in.",
                    
                    };
                    await _notificationService.AddNotificationAsync(notification);

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Login");
            }
        }

        [HttpGet("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return LocalRedirect("~/");
        }
    }
}
