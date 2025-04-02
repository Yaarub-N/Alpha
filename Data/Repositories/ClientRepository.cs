

using Data.Contexts;
using Data.Entities;
using Data.IRepositories;
using Domain.Models;

namespace Data.Repositories;

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity, Client>(context), IClientRepository
{

}
