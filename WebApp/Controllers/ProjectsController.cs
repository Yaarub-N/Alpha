using Business.Interfaces;
using Business.Models;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService, IClientService clientService, IStatusService statusService, IUserService userService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IStatusService _statusService = statusService;
    private readonly IUserService _userService = userService;
    private static readonly string[] data = ["Update failed."];

    public async Task<IActionResult> Index()
    {
        var viewModel = new ProjectsViewModel()
        {
            Projects = await SetProjectsAsync(),
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = await SetClientSelectListItemsAsync(),
                Members = await SetUserSelectListItemsAsync(),
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = await SetClientSelectListItemsAsync(),
                Members = await SetUserSelectListItemsAsync(),
                Statuses = await SetStatusSelectListItemsAsync()
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { errors });
        }

        var dto = new AddProjectForm
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = model.ProjectName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId,
            UserId = model.UserId,
            StatusId = 1
        };

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var extension = Path.GetExtension(model.ImageFile.FileName);
            var filePath = Path.Combine("wwwroot/images/projects", fileName + extension);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }
            dto.ImageUrl = "/images/projects/" + fileName + extension;
        }
        else
        {
            dto.ImageUrl = "/images/projects/project-template.svg";
        }

        var result = await _projectService.AddProjectAsync(dto);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        var createdProject = await _projectService.GetProjectByIdAsync(dto.Id);

        var notificationService = HttpContext.RequestServices.GetRequiredService<INotificationService>();
        await notificationService.AddNotificationAsync(new NotificationFormData
        {
            NotificationTypeId = 2,
            NotificationTargetId = 1,
            Message = $"New project '{model.ProjectName}' has been created",
            Image = dto.ImageUrl
        });

        return PartialView("~/Views/Shared/ListItems/_ProjectListItemPartial.cshtml", createdProject.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateProjectForm model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var extension = Path.GetExtension(model.ImageFile.FileName);
            var filePath = Path.Combine("wwwroot/images/projects", fileName + extension);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            model.ImageUrl = "/images/projects/" + fileName + extension;
        }

        var success = await _projectService.UpdateProjectAsync(model);
        if (!success.Succeeded)
        {
            return BadRequest(new { error = success.ErrorMessage });
        }

        return Json(new { success = true });
    }

    private async Task<IEnumerable<Project>> SetProjectsAsync()
    {
        var projectResult = await _projectService.GetProjectAsync();
        var projects = projectResult?.Result ?? [];
        return projects.OrderByDescending(x => x.Created);
    }

    private async Task<IEnumerable<SelectListItem>> SetClientSelectListItemsAsync()
    {
        var clientResult = await _clientService.GetClientsAsync();
        var clients = clientResult?.Result ?? [];
        clients = clients.OrderBy(x => x.ClientName);

        return clients.Select(client => new SelectListItem
        {
            Value = client.Id,
            Text = client.ClientName
        });
    }

    private async Task<IEnumerable<SelectListItem>> SetUserSelectListItemsAsync()
    {
        var userResult = await _userService.GetAllUsersAsync();
        var users = userResult?.Result ?? [];

        return users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
            .Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = $"{user.FirstName} {user.LastName}"
            });
    }

    private async Task<IEnumerable<SelectListItem>> SetStatusSelectListItemsAsync()
    {
        var statusResult = await _statusService.GetStatusAsync();
        var statuses = statusResult?.Result ?? [];

        return statuses.OrderBy(x => x.Id)
            .Select(status => new SelectListItem
            {
                Value = status.Id.ToString(),
                Text = status.StatusName
            });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _projectService.DeleteProjectAsync(new Project { Id = id });
        if (result.Succeeded)
            return RedirectToAction("Index");

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(string projectId, string userId)
    {
        var result = await _projectService.AddMemberToProjectAsync(projectId, userId);
        if (result.Succeeded)
            return Json(new { success = true });

        return Json(new { success = false, error = result.ErrorMessage });
    }

    [HttpGet]
    public async Task<IActionResult> GetProject(string id)
    {
        var result = await _projectService.GetProjectByIdAsync(id);
        if (!result.Succeeded || result.Result == null)
            return Json(new { success = false });

        return Json(new { success = true, project = result.Result });
    }
}
