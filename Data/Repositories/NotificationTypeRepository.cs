using Data.Contexts;
using Data.Entitites;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class NotificationTypeRepository(AppDbContext context) : BaseRepository<NotificationTypeEntity, NotificationType>(context), INotificationTypeRepository
{

}
