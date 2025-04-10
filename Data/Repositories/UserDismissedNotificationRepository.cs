using Data.Contexts;
using Data.Entities;
using Data.Entitites;
using Data.Interfaces;
using Domain.Models;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserDismissedNotificationRepository(AppDbContext context) : BaseRepository<UserDismissedNotificationEntity, UserDismissedNotification>(context), IUserDismissedNotificationRepository
{
    public async Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId)
    {
        var ids = await _table.Where(x => x.UserId == userId).Select(x => x.NotificationId).ToListAsync();
        return new RepositoryResult<IEnumerable<string>> { Succeeded = true , statusCode = 200, Result = ids};
    }
}