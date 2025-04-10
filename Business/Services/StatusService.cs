

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{

    private readonly IStatusRepository _statusRepository = statusRepository;
    public async Task<StatusResult> GetStatusAsync()
    {
        var repositoryResult = await _statusRepository.GetAllAsync();

        if (!repositoryResult.Succeeded || repositoryResult.Result == null)
        {
            return new StatusResult
            {
                Succeeded = false,
                StatusCode = repositoryResult.statusCode,
                ErrorMessage = repositoryResult.ErrorMessage
            };
        }

        return new StatusResult
        {
            Succeeded = true,
            StatusCode = 200,
            Result = repositoryResult.Result
        };
    }


    public async Task<SingleStatusResult> GetStatusByIdAsync(int id)
    {
        if (id <= 0)
            return new SingleStatusResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Id can't be less than or equal to 0." };

        var result = await _statusRepository.GetAsync(x => x.Id == id);
        return result.MapTo<SingleStatusResult>();
    }



    public async Task<StatusResult> UpdateStatusAsync(Status status)
    {
        if (status == null)
            return new StatusResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Status can't be null." };
        var entity = status.MapTo<StatusEntity>();
        var result = await _statusRepository.UpdateAsync(entity);
        return result.MapTo<StatusResult>();
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
