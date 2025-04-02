using Data.Entities;
using Domain.Models;

namespace Data.IRepositories;

public interface IStatusRepository : IBaseRepository<StatusEntity, Status>
{
}
