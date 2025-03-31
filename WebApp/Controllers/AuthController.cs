using Business.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.AuthModels.RegisterModels;



namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        [HttpPost]
        public IActionResult Login(LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form); // Returnera formuläret med felmeddelanden
            }
            // Fortsätta med inloggningslogik
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register( RegisterFormModel form)
        {
            // Lägg till ett explicit fel
            ModelState.AddModelError("", "Test error message");

            if (!ModelState.IsValid)
            {
                return View(form);
            }
            return RedirectToAction("Login");
        } 
        
      
    }
}
