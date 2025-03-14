using Microsoft.AspNetCore.Mvc;
using WebApp.Models.AuthModels.RegisterModels;


namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
       
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
           
            return View(form);
        } 
        
      
    }
}
