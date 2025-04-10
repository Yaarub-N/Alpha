using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class ProjectUpdateFactory
{
    public static void Update(ProjectEntity entity, UpdateProjectForm form)
    {
        entity.ProjectName = form.ProjectName;
        entity.Description = form.Description;
        entity.StartDate = form.StartDate ?? entity.StartDate;
        entity.EndDate = form.EndDate ?? entity.EndDate;
        entity.Budget = form.Budget;
        entity.ClientId = form.ClientId;
        entity.UserId = form.UserId;
        entity.StatusId = form.StatusId;

        if (!string.IsNullOrWhiteSpace(form.ImageUrl))
            entity.ImageUrl = form.ImageUrl;
    }
}
