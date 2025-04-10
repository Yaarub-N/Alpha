using Data.Contexts;
using Data.Entitites;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class NotificationTargetRepository(AppDbContext context) : BaseRepository<NotificationTargetEntity, NotificationTarget>(context) , INotificationTargetRepository
{

}
