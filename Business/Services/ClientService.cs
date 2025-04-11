using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult<bool>> AddClientAsync(Client client)
    {
        client.Id = Guid.NewGuid().ToString();
        var entity = client.MapTo<ClientEntity>();
        var result = await _clientRepository.AddAsync(entity);

        return new ClientResult<bool>
        {
          
            Succeeded = result.Succeeded,
            StatusCode = result.statusCode,
            Result = result.Result,
            ErrorMessage = result.ErrorMessage
        };
    }



    public async Task<ClientResult<IEnumerable<Client>>> GetClientsAsync()
    {
        var repositoryResult = await _clientRepository.GetAllAsync
            (
                orderByDescending: false,
                sortBy: x => x.ClientName
            );

        var entities = repositoryResult.Result;
        var clients = entities?.Select(entity => entity.MapTo<Client>()) ?? [];

        return new ClientResult<IEnumerable<Client>> { Succeeded = true, StatusCode = 200, Result = clients };
    }

    public async Task<ClientResult<Client>> GetClientByIdAsync(string id)
    {
        var repositoryResult = await _clientRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new ClientResult<Client> { Succeeded = false, StatusCode = 404, ErrorMessage = $"Client with id '{id}' was not found." };

        var client = entity.MapTo<Client>();
        return new ClientResult<Client> { Succeeded = true, StatusCode = 200, Result = client };
    }
}
