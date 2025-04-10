using Domain.Models;
using Data.Entities;

namespace Business.Extensions;

public static class MappingExtensions
{
    public static ProjectEntity ToEntity(this Project project)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        return new ProjectEntity
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ImageUrl = project.ImageUrl,
            Description = project.Description,
            StartDate = project.StartDate ?? DateTime.MinValue, // fallback om null ChatGpt 4o
            EndDate = project.EndDate,
            Budget = project.Budget,
            Created = project.Created,
            ClientId = project.Client?.Id ?? "", 
            UserId = project.User?.Id ?? "",
            StatusId = project.Status?.Id ?? 0
        };
    }
}
