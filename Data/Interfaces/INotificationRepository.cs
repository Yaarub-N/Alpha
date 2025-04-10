using Data.Entitites;
using Domain.Models;
using Domain.Responses;

namespace Data.Interfaces
{
    public interface INotificationRepository : IBaseRepository<NotificationEntity, Notification>
    {
        Task<NotificationResult<Notification>> GetLatestNotification();
    }
}