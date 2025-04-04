using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ProjectsController(IProjectService projectService) : Controller
    {
        private readonly IProjectService _projectService = projectService;





        [Route("admin/projects")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public IActionResult Delete()
        {

            return Ok();
        }


        [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }
        [HttpPut]
        public IActionResult Put()
        {
            return Ok();
        }


    }
}
