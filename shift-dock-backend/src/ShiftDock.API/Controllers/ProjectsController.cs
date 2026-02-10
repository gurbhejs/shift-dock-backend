using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Projects;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages project and shift operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Gets all projects in an organization
    /// </summary>
    /// <param name="organizationId">Organization ID from query string</param>
    /// <returns>List of projects</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponse>>>> GetProjects([FromQuery] string organizationId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.GetProjectsByOrganizationAsync(Guid.Parse(organizationId), Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets details of a specific project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{projectId}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> GetProject(Guid projectId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.GetProjectByIdAsync(projectId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="request">Project details including organizationId</param>
    /// <returns>Created project</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> CreateProject([FromBody] CreateProjectWithOrgRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.CreateProjectAsync(Guid.Parse(request.OrganizationId), Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="request">Updated project details</param>
    /// <returns>Updated project</returns>
    [HttpPut("{projectId}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> UpdateProject(Guid projectId, [FromBody] UpdateProjectRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.UpdateProjectAsync(projectId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{projectId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProject(Guid projectId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.DeleteProjectAsync(projectId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets shifts for a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of shifts</returns>
    [HttpGet("{projectId}/shifts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ShiftDto>>>> GetShifts(Guid projectId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.GetShiftsByProjectAsync(projectId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Creates a shift for a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="request">Shift details</param>
    /// <returns>Created shift</returns>
    [HttpPost("{projectId}/shifts")]
    public async Task<ActionResult<ApiResponse<ShiftDto>>> CreateShift(Guid projectId, [FromBody] CreateShiftRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.CreateShiftAsync(projectId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Syncs shifts for a project (replaces all shifts with the provided list)
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="request">List of shifts to sync</param>
    /// <returns>All shifts after sync</returns>
    [HttpPut("{projectId}/shifts/sync")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ShiftDto>>>> SyncShifts(Guid projectId, [FromBody] SyncShiftsRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.SyncShiftsAsync(projectId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Updates a shift
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="request">Updated shift details</param>
    /// <returns>Updated shift</returns>
    [HttpPut("{projectId}/shifts/{shiftId}")]
    public async Task<ActionResult<ApiResponse<ShiftDto>>> UpdateShift(Guid projectId, Guid shiftId, [FromBody] UpdateShiftRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.UpdateShiftAsync(projectId, shiftId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a shift
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="shiftId">Shift ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{projectId}/shifts/{shiftId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteShift(Guid projectId, Guid shiftId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _projectService.DeleteShiftAsync(projectId, shiftId, Guid.Parse(userId));
        return Ok(result);
    }
}
