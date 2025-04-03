

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.IRepositories;
using Domain.Extentions;
using Domain.Models;
using System.Linq.Expressions;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, IStatusService statusRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusServices = statusRepository;

    public async Task<ProjectResult> AddProjectAsync(AddProjectForm project)
    {
        if (project == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Project can't be null." };
        var entity = project.MapTo<ProjectEntity>();
        var statusResult = await _statusServices.GetStatusByIdAsync(project.StatusId);
        if (!statusResult.Succeeded)
            return new ProjectResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Status not found." };
        var status = statusResult.Result!.FirstOrDefault();
        if (status == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Status not found." };

        entity.StatusId = status.Id;

        var result = await _projectRepository.AddAsync(entity);
        return result.MapTo<ProjectResult>();
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectAsync()
    {
        var projects = await _projectRepository.GetAllAsync(
       orderByDescending: true,
       sortBy: x => x.Created,
       where: null,

       //Params förväntar sig en lista av lambda-uttryck – inte flera includes => som du skriver.
       includes:
       [
        x => x.Client,
        x => x.User,
        x => x.Status
       ]
   );



        return projects.MapTo<ProjectResult<IEnumerable<Project>>>();
    }

    public async Task<ProjectResult<Project>> GetProjectByIdAsync(string id)
    {
        var project = await _projectRepository.GetAsync(
            where: x => x.Id == id,
            includes:
            [
                x => x.Client,
                x => x.User,
                x => x.Status
            ]
        );
        return project.MapTo<ProjectResult<Project>>();
    }

    public async Task<ProjectResult> UpdateProjectAsync(Project project)
    {
        var entity = project.MapTo<ProjectEntity>();
        var result = await _projectRepository.UpdateAsync(entity);
        return result.MapTo<ProjectResult>();
    }


    //varför vill jag skick hela projectet om ´jag kan ta bort det bara med id?

    public async Task<ProjectResult> DeleteProjectAsync(Project project)
    {
        var entity = project.MapTo<ProjectEntity>();
        var result = await _projectRepository.RemoveAsync(entity);
        return result.MapTo<ProjectResult>();
    }
}
