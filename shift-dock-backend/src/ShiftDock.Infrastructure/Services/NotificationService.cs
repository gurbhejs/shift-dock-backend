using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;

namespace ShiftDock.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var notification = new Notification
        {
            UserId = userId.ToString(),
            Type = "General",
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Implement push notification logic here (Firebase, OneSignal, etc.)
    }

    public async Task SendBulkNotificationsAsync(IEnumerable<Guid> userIds, string title, string message, CancellationToken cancellationToken = default)
    {
        var notifications = new List<Notification>();

        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
            if (user != null)
            {
                notifications.Add(new Notification
                {
                    UserId = userId.ToString(),
                    Type = "General",
                    Title = title,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (notifications.Any())
        {
            await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // TODO: Implement bulk push notification logic here
        }
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId.ToString(), cancellationToken);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId.ToString(), cancellationToken);
        if (notification == null)
        {
            throw new KeyNotFoundException($"Notification with ID {notificationId} not found");
        }

        notification.IsRead = true;

        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<object>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetUserNotificationsAsync(userId.ToString(), cancellationToken);
        return notifications.Select(n => new { n.Id, n.Type, n.Title, n.Message, n.IsRead, n.CreatedAt });
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId.ToString(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
