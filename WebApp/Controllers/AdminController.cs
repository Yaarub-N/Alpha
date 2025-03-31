using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{

    [Route("admin/overview")]
    public class AdminController : Controller
    {
        public IActionResult Clients()
        {
            return View();
        }
    }
}
