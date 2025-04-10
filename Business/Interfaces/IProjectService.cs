using Business.Models;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResult> AddProjectAsync(AddProjectForm project);
        Task<ProjectResult> DeleteProjectAsync(Project project);
        Task<ProjectResult<IEnumerable<Project>>> GetProjectAsync();
        Task<ProjectResult<Project>> GetProjectByIdAsync(string id);
        Task<ProjectResult> AddMemberToProjectAsync(string projectId, string userId);
        Task<RepositoryResult<bool>> UpdateProjectAsync(UpdateProjectForm form);
    }
}