using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Reports;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Generates and provides access to various reports
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Gets payroll report for workers in an organization
    /// </summary>
    /// <param name="request">Report parameters including organization ID and date range</param>
    /// <returns>Worker payroll data</returns>
    [HttpPost("payroll")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkerPayrollResponse>>>> GetPayrollReport([FromBody] PayrollReportRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _reportService.GetPayrollReportAsync(request, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets project report for an organization
    /// </summary>
    /// <param name="request">Report parameters including organization ID and filters</param>
    /// <returns>Project statistics</returns>
    [HttpPost("projects")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectReportResponse>>>> GetProjectReport([FromBody] ProjectReportRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _reportService.GetProjectReportAsync(request, Guid.Parse(userId));
        return Ok(result);
    }
}
