﻿using Data.Contexts;
using Data.Entitites;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;


public class NotificationRepository(AppDbContext context) : BaseRepository<NotificationEntity, Notification>(context), INotificationRepository
{
    public async Task<NotificationResult<Notification>> GetLatestNotification()
    {
        var entity = await _table.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync();
        var notification = entity!.MapTo<Notification>();
        return new NotificationResult<Notification> { Succeeded = true, StatusCode = 200, Result = notification };
    }
}
