using Business.Interfaces;
using Business.Models;
using Domain.Extentions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models.AuthModels.RegisterModels;



namespace WebApp.Controllers;


public class AuthController(IAuthService authService, IUserService userService) : Controller
{

    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;

    [HttpPost]
    public  async Task<IActionResult> Login(SignInFromModel model, string returnUrl = "~/")
    {


        if (ModelState.IsValid)
        {
            var signInFormData = model.MapTo<SignInFormData>();
            var authResult = await _authService.SignInAsync(signInFormData);

            if (authResult.Succeeded)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userResult = await _userService.GetUserByIdAsync(userId!);
                var user = userResult.Result;

                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "Unable to login. Try another email or password.";
        return View(model);
    }

    public IActionResult Login(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";

        return View();
    }

    public IActionResult Register(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";

        return View();
    }

    [HttpPost]
    public async Task< IActionResult> Register(RegisterFormModel model, string returnUrl = "~/")
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;  
            ViewBag.ErrorMessage = "";
            return View(model);
        }

        var signUpFormData = model.MapTo<SignUpFormData>();
        var authResult = await _authService.SignUpAsync(signUpFormData);

        if (authResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = authResult.ErrorMessage;
        return View(model);
    }


    [Route("auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }


}
