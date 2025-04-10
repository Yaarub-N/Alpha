

using Data.Entities;
using Data.Entitites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity>(options)
{

    public DbSet<ProjectEntity> Projects { get; set; } 
    public DbSet<ClientEntity> Clients { get; set; } 
    public DbSet<StatusEntity> Statuses { get; set; }   
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public DbSet<NotificationTargetEntity> NotificationTargets { get; set; }
    public DbSet<UserDismissedNotificationEntity> DismissedNotifications { get; set; }
}
