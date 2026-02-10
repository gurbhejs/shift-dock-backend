using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Assignments;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages worker assignments to shifts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    /// <summary>
    /// Gets all assignments for a shift
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <returns>List of assignments</returns>
    [HttpGet("shift/{shiftId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkerAssignmentResponse>>>> GetShiftAssignments(Guid shiftId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.GetAssignmentsByShiftAsync(shiftId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets all assignments for current user
    /// </summary>
    /// <returns>List of user's assignments</returns>
    [HttpGet("my-assignments")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkerAssignmentResponse>>>> GetMyAssignments()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.GetMyAssignmentsAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Assigns a worker to a shift
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="request">Assignment details with userId</param>
    /// <returns>Created assignment</returns>
    [HttpPost("shift/{shiftId}/assign")]
    public async Task<ActionResult<ApiResponse<WorkerAssignmentResponse>>> AssignWorker(Guid shiftId, [FromBody] CreateAssignmentRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.AssignWorkerToShiftAsync(shiftId, request.UserId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Bulk assigns multiple workers to a shift
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="request">List of user IDs to assign</param>
    /// <returns>List of created assignments</returns>
    [HttpPost("shift/{shiftId}/bulk-assign")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkerAssignmentResponse>>>> BulkAssignWorkers(Guid shiftId, [FromBody] BulkAssignWorkersRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.BulkAssignWorkersAsync(shiftId, request.UserIds, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Updates assignment status
    /// </summary>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="request">Status update details</param>
    /// <returns>Updated assignment</returns>
    [HttpPut("{assignmentId}/status")]
    public async Task<ActionResult<ApiResponse<WorkerAssignmentResponse>>> UpdateAssignmentStatus(Guid assignmentId, [FromBody] UpdateAssignmentStatusRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.UpdateAssignmentStatusAsync(assignmentId, request, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Removes an assignment
    /// </summary>
    /// <param name="assignmentId">Assignment ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{assignmentId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveAssignment(Guid assignmentId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _assignmentService.RemoveAssignmentAsync(assignmentId, Guid.Parse(userId));
        return Ok(result);
    }
}
