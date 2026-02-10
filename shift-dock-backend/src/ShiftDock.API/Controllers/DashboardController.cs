using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Dashboard;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Provides dashboard statistics and overview data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Gets organization dashboard statistics
    /// </summary>
    /// <param name="orgId">Organization ID from route</param>
    /// <returns>Dashboard statistics</returns>
    [HttpGet("stats/{orgId}")]
    public async Task<ActionResult<ApiResponse<DashboardStatsResponse>>> GetStats(Guid orgId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _dashboardService.GetDashboardStatsAsync(orgId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets worker-specific dashboard data
    /// </summary>
    /// <param name="orgId">Organization ID from route</param>
    /// <returns>Worker dashboard data</returns>
    [HttpGet("worker/{orgId}")]
    public async Task<ActionResult<ApiResponse<WorkerDashboardResponse>>> GetWorkerDashboard(Guid orgId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _dashboardService.GetWorkerDashboardAsync(orgId, Guid.Parse(userId));
        return Ok(result);
    }
}
