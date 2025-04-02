using Business.Models;
using Domain.Models;

namespace Business.Interfaces
{
    public interface IStatusService
    {
        Task<StatusResult> AddStatusAsync(Status status);
        Task<StatusResult> GetStatusAsync();
    }
}