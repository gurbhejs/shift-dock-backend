using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages user settings and preferences
/// </summary>
[ApiController]
[Route("api/users/me")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IUserService _userService;

    public SettingsController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets the current user's notification preferences
    /// </summary>
    /// <returns>Notification preferences</returns>
    [HttpGet("notification-preferences")]
    public async Task<ActionResult<ApiResponse<object>>> GetNotificationPreferences()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _userService.GetNotificationPreferencesAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Updates the current user's notification preferences
    /// </summary>
    /// <param name="request">Updated notification preferences</param>
    /// <returns>Updated preferences</returns>
    [HttpPut("notification-preferences")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateNotificationPreferences([FromBody] object request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _userService.UpdateNotificationPreferencesAsync(Guid.Parse(userId), request);
        return Ok(result);
    }
}
