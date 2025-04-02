

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.IRepositories;
using Domain.Extentions;
using Domain.Models;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{

    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult> GetStatusAsync()
    {
        var result = await _statusRepository.GetAllAsync();


        return result.MapTo<StatusResult>();

        //result.Succeeded
        //? new StatusResult { Result = result.Result, Succeeded = true, statusCode = result.statusCode }
        //: new StatusResult { ErrorMessage = result.ErrorMessage, Succeeded = false, statusCode = result.statusCode };
    }

    public async Task<StatusResult> AddStatusAsync(Status status)
    {


        //chatGpt 4o
        var entity = status.MapTo<StatusEntity>();         // Mappa till rätt EF-typ
        var result = await _statusRepository.AddAsync(entity);  // Lägg till i DB
        return result.MapTo<StatusResult>();
        //return result.Succeeded
        //    ? new StatusResult { Result = result.Result, Succeeded = true, statusCode = result.statusCode }
        //    : new StatusResult { ErrorMessage = result.ErrorMessage, Succeeded = false, statusCode = result.statusCode };
    }
}
