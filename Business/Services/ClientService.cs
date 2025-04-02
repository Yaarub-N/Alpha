

using Business.Interfaces;
using Data.Entities;
using Data.IRepositories;
using Domain.Extentions;
using Domain.Models;

namespace Business.Models.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> AddClientAsync(Client client)
    {
        var entity = client.MapTo<ClientEntity>();
        var result = await _clientRepository.AddAsync(entity);
        return result.MapTo<ClientResult>();
    }

    public async Task<ClientResult> GetClientAsync()
    {
        var result = await _clientRepository.GetAllAsync();

        return result.MapTo<ClientResult>();

    }
}
