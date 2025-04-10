using Data.Entities;
using Domain.Models;

namespace Business.Extensions;

public static class ProjectExtensions
{
    public static ProjectEntity ToEntity(this Project model)
    {
        return new ProjectEntity
        {
            Id = model.Id,
            ProjectName = model.ProjectName,
            ImageUrl = model.ImageUrl,
            Description = model.Description,
            StartDate = model.StartDate ?? DateTime.Now,
            EndDate = model.EndDate,
            Budget = model.Budget,
            Created = model.Created,
            ClientId = model.Client.Id,
            UserId = model.User.Id,
            StatusId = model.Status.Id
        };
    }
}
