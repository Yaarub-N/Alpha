using Business.Models;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces
{
    public interface IStatusService
    {
        Task<StatusResult> AddStatusAsync(Status status);
        Task<StatusResult> GetStatusAsync();
        Task<SingleStatusResult> GetStatusByIdAsync(int id);
    }
}