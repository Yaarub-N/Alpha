

using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
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

        if (!statusResult.Succeeded || statusResult.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Status not found." };

        var status = statusResult.Result;
        if (status == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Status not found." };

        entity.StatusId = status.Id; 
        var result = await _projectRepository.AddAsync(entity);
        return result.MapTo<ProjectResult>();
    }
    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectAsync()
    {
        var entities = await _projectRepository.GetAllAsync(
            orderByDescending: true,
            sortBy: x => x.Created,
            includes: [
                x => x.Client,
            x => x.User,
            x => x.Status
            ]
        );

        if (!entities.Succeeded || entities.Result == null)
        {
            return new ProjectResult<IEnumerable<Project>> { Succeeded = false, StatusCode = 500, ErrorMessage = "Could not fetch projects."};
        }

        return new ProjectResult<IEnumerable<Project>>{ Succeeded = true,StatusCode = 200, Result = entities.Result };
    }


    public async Task<ProjectResult<Project>> GetProjectByIdAsync(string id)
    {
        var result = await _projectRepository.GetAsync(
            where: x => x.Id == id,
            includes: [
                x => x.Client,
            x => x.User,
            x => x.Status
            ]
        );

        if (!result.Succeeded || result.Result == null)
            return new ProjectResult<Project> {  Succeeded = false, StatusCode = result.statusCode, ErrorMessage = result.ErrorMessage};

        return new ProjectResult<Project>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = result.Result  
        };
    }

    public async Task<RepositoryResult<bool>> UpdateProjectAsync(UpdateProjectForm form)
    {
        var existingResult = await _projectRepository.GetAsync(x => x.Id == form.Id);
        if (!existingResult.Succeeded || existingResult.Result == null)
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 404, ErrorMessage = "Project not found." };
        var entity = existingResult.Result.MapTo<ProjectEntity>();
        ProjectUpdateFactory.Update(entity, form);

        return await _projectRepository.UpdateAsync(entity);
    }


    public async Task<ProjectResult> DeleteProjectAsync(Project project)
    {
        var entity = project.MapTo<ProjectEntity>();
        var result = await _projectRepository.RemoveAsync(entity);
        return result.MapTo<ProjectResult>();
    }

    public async Task<ProjectResult> AddMemberToProjectAsync(string projectId, string userId)
    {
        var projectResult = await _projectRepository.GetAsync(x => x.Id == projectId);
        if (!projectResult.Succeeded || projectResult.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, ErrorMessage = "Project not found." };

        var projectEntity = projectResult.Result.MapTo<ProjectEntity>();
        projectEntity.UserId = userId;

        var updateResult = await _projectRepository.UpdateAsync(projectEntity);

        return updateResult.MapTo<ProjectResult>();
    }
}
