using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IClientService
{
   
    Task<ClientResult<bool>> AddClientAsync(Client client);
    Task<ClientResult<Client >> GetClientByIdAsync(string id);
    Task<ClientResult<IEnumerable<Client>>> GetClientsAsync();
}
