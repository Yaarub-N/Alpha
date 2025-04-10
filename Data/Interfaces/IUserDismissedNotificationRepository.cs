using Data.Entities;
using Domain.Models;
using Domain.Responses;

namespace Data.Interfaces;

public interface IUserDismissedNotificationRepository : IBaseRepository<UserDismissedNotificationEntity, UserDismissedNotification>
{
    Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId);
}
