using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Users;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages user profile operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    /// <returns>User profile</returns>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetProfile()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _userService.GetUserByIdAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Updates the current user's profile
    /// </summary>
    /// <param name="request">Updated user details</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _userService.UpdateUserAsync(Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUser(Guid userId)
    {
        var currentUserId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _userService.GetUserByIdAsync(userId);
        return Ok(result);
    }
}
