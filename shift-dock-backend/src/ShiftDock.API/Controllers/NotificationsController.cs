using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages user notifications
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Gets all notifications for the current user
    /// </summary>
    /// <returns>List of notifications</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetNotifications()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.GetUserNotificationsAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Marks a specific notification as read
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Success message</returns>
    [HttpPut("{notificationId}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid notificationId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.MarkAsReadAsync(notificationId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Marks all notifications as read for the current user
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.MarkAllAsReadAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets unread notification count for the current user
    /// </summary>
    /// <returns>Count of unread notifications</returns>
    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<object>>> GetUnreadCount()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.GetUnreadCountAsync(Guid.Parse(userId));
        return Ok(result);
    }
}
