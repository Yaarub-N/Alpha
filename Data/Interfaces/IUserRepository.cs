using Data.Entities;
using Domain.Models;

namespace Data.IRepositories;

public interface IUserRepository : IBaseRepository<UserEntity, User>
{
}
