

using Data.Contexts;
using Data.Entities;
using Data.IRepositories;
using Domain.Models;

namespace Data.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity, User>(context), IUserRepository
{
}
