using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Entitites;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Models;
using Domain.Responses;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;

namespace Business.Services;

public interface INotificationService
{
    Task<NotificationResult> AddNotificationAsync(NotificationFormData formData);
    Task DismissNotificationAsync(string notificationId, string userId);
    Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10);
}

public class NotificationService(
    INotificationRepository notificationRepository,
    INotificationTypeRepository notificationTypeRepository,
    INotificationTargetRepository notificationTargetRepository,
    IUserDismissedNotificationRepository userDismissedNotificationRepository,
    IHubContext<NotificationHub> notificationHub
) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationTypeRepository _notificationTypeRepository = notificationTypeRepository;
    private readonly INotificationTargetRepository _notificationTargetRepository = notificationTargetRepository;
    private readonly IUserDismissedNotificationRepository _userDismissedNotificationRepository = userDismissedNotificationRepository;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    public async Task<NotificationResult> AddNotificationAsync(NotificationFormData formData)
    {
        if (formData == null)
            return new NotificationResult { Succeeded = false, StatusCode = 400, ErrorMessage = "Invalid form data." };

        // Standardbild beroende på typ
        if (string.IsNullOrEmpty(formData.Image))
        {
            formData.Image = formData.NotificationTypeId switch
            {
                1 => "/images/profiles/user-template.svg",
                2 => "/images/projects/project-template.svg",
                _ => "/images/default.svg"
            };
        }

        var notificationEntity = formData.MapTo<NotificationEntity>();
        var result = await _notificationRepository.AddAsync(notificationEntity);

        if (result.Succeeded)
        {
            var latestNotification = await _notificationRepository.GetLatestNotification();
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", latestNotification.Result);
        }

        return result.Succeeded
            ? new NotificationResult { Succeeded = true, StatusCode = 200 }
            : new NotificationResult { Succeeded = false, StatusCode = result.statusCode, ErrorMessage = result.ErrorMessage };
    }

    public async Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10)
    {
        const string adminTargetName = "Admin";

        var dismissedResult = await _userDismissedNotificationRepository.GetNotificationsIdsAsync(userId);
        var dismissedIds = dismissedResult.Result ?? [];
        var where = string.Equals(roleName, adminTargetName, StringComparison.OrdinalIgnoreCase)
            ? (Expression<Func<NotificationEntity, bool>>)(x => !dismissedIds.Contains(x.Id))
            : x => !dismissedIds.Contains(x.Id) && x.NotificationTarget.TargetName != adminTargetName;
        //Expression<Func<NotificationEntity, bool>> where = x => !dismissedIds.Contains(x.Id);

        var result = await _notificationRepository.GetAllAsync(
            orderByDescending: true,
            sortBy: x => x.CreateDate,
            where: where,
            includes: x => x.NotificationTarget
        );

        if (!result.Succeeded)
            return new NotificationResult<IEnumerable<Notification>> { Succeeded = false, StatusCode = 404, ErrorMessage = result.ErrorMessage };

        var notifications = result.Result!.Select(x => x.MapTo<Notification>());
        return new NotificationResult<IEnumerable<Notification>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = notifications
        };
    }

    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var exists = await _userDismissedNotificationRepository.ExistAsync(x =>
            x.NotificationId == notificationId && x.UserId == userId);

        if (!exists.Result)
        {
            var entity = new UserDismissedNotificationEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };

            await _userDismissedNotificationRepository.AddAsync(entity);
            await _notificationHub.Clients.User(userId).SendAsync("NotificationDismissed", notificationId);
        }
    }
}
