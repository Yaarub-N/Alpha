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
            return Json(new { success = false, errors });
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
            StatusId = 1 // standard-status om inget annat anges
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

        if (result != null && result.Succeeded)
        {
            var createdProject = await _projectService.GetProjectByIdAsync(dto.Id);

            // Create notification for the new project
            var notificationService = HttpContext.RequestServices.GetRequiredService<INotificationService>();
            await notificationService.AddNotificationAsync(new NotificationFormData
            {
                NotificationTypeId = 2, // Assuming 2 is for project notifications based on your code
                NotificationTargetId = 1, // Set appropriate target ID
                Message = $"New project '{model.ProjectName}' has been created",
                Image = dto.ImageUrl
            });

            return Json(new
            {
                success = true,
                project = createdProject.Result
            });
        }

        return Json(new { success = false, error = "Unable to create the project." });
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

            return Json(new { success = false, errors });
        }

        // Chat Gpt4o
        // Handle image upload if a new image is provided
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
        if (success.Succeeded)
            return Json(new { success = true });

        return Json(new { success = false, errors = new { General = data } });
    }



    private async Task<IEnumerable<Project>> SetProjectsAsync()
    {
        var projectResult = await _projectService.GetProjectAsync();

        //chat Gpt4o

        // This was the source of the error - attempting to call SelectMany on a collection
        // that is already of the correct type
        var projects = projectResult?.Result ?? [];

        return projects.OrderByDescending(x => x.Created);
    }


    private async Task<IEnumerable<SelectListItem>> SetClientSelectListItemsAsync()
    {
        var clientResult = await _clientService.GetClientsAsync();
        var clients = clientResult?.Result ?? [];
        clients = clients.OrderBy(x => x.ClientName);

        var selectListItems = clients.Select(client => new SelectListItem
        {
            Value = client.Id,
            Text = client.ClientName
        });

        return selectListItems;
    }

    private async Task<IEnumerable<SelectListItem>> SetUserSelectListItemsAsync()
    {
        var userResult = await _userService.GetAllUsersAsync();
        var users = userResult?.Result ?? [];

        users = users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);

        var selectListItems = users.Select(user => new SelectListItem
        {
            Value = user.Id,
            Text = $"{user.FirstName} {user.LastName}"
        });

        return selectListItems;
    }

    private async Task<IEnumerable<SelectListItem>> SetStatusSelectListItemsAsync()
    {
        var statusResult = await _statusService.GetStatusAsync();
        var statuses = statusResult?.Result ?? [];
        statuses = statuses.OrderBy(x => x.Id);

        var selectListItems = statuses.Select(status => new SelectListItem
        {
            Value = status.Id.ToString(),
            Text = status.StatusName
        });

        return selectListItems;
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
        // validera + uppdatera projektet i databasen
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

        return Json(new
        {
            success = true,
            project = result.Result
        });
    }




}


