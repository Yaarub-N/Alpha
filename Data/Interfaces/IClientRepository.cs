using Data.Entities;
using Domain.Models;

namespace Data.IRepositories;

public interface IClientRepository : IBaseRepository<ClientEntity, Client>
{
}
