using Business.Models;
using Domain.Models;

namespace Business.Interfaces
{
    public interface IClientService
    {
        Task<ClientResult> AddClientAsync(Client client);
        Task<ClientResult> GetClientAsync();
    }
}