

using Data.Contexts;
using Data.Entities;
using Data.IRepositories;
using Domain.Models;

namespace Data.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectRepository
{
}
