using Data.Entities;
using Domain.Models;

namespace Data.IRepositories;

public interface IProjectRepository : IBaseRepository<ProjectEntity, Project>
{
}
