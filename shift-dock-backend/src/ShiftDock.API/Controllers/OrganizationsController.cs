using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Organizations;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Manages organization operations including creation, updates, and join requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    /// <summary>
    /// Creates a new organization
    /// </summary>
    /// <param name="request">Organization details</param>
    /// <returns>Created organization</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.CreateOrganizationAsync(Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Gets all organizations where the current user is a member
    /// </summary>
    /// <returns>List of organizations</returns>
    [HttpGet("my-organizations")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrganizationResponse>>>> GetMyOrganizations()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.GetUserOrganizationsAsync(Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets details of a specific organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <returns>Organization details</returns>
    [HttpGet("{orgId}")]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> GetOrganization(Guid orgId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.GetOrganizationByIdAsync(orgId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Updates an organization (owner/admin only)
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="request">Updated organization details</param>
    /// <returns>Updated organization</returns>
    [HttpPut("{orgId}")]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> UpdateOrganization(Guid orgId, [FromBody] UpdateOrganizationRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.UpdateOrganizationAsync(orgId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Submits a request to join an organization
    /// </summary>
    /// <param name="request">Join request details including organization code</param>
    /// <returns>Success message</returns>
    [HttpPost("join")]
    public async Task<ActionResult<ApiResponse<object>>> JoinOrganization([FromBody] JoinOrganizationRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.JoinOrganizationAsync(Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Gets all pending join requests for an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <returns>List of join requests</returns>
    [HttpGet("{orgId}/join-requests")]
    public async Task<ActionResult<ApiResponse<IEnumerable<JoinRequestResponse>>>> GetJoinRequests(Guid orgId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.GetJoinRequestsAsync(orgId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Approves or rejects multiple join requests
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="request">List of request IDs and approval decision</param>
    /// <returns>Success message with processing details</returns>
    [HttpPost("{orgId}/join-requests/handle")]
    public async Task<ActionResult<ApiResponse<HandleJoinRequestResponse>>> HandleJoinRequest(Guid orgId, [FromBody] HandleJoinRequestRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.HandleJoinRequestsAsync(orgId, Guid.Parse(userId), request);
        return Ok(result);
    }

    /// <summary>
    /// Gets all members of an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <returns>List of organization members</returns>
    [HttpGet("{orgId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetMembers(Guid orgId)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _organizationService.GetMembersAsync(orgId, Guid.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// Updates a member's role in an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="userId">User ID to update</param>
    /// <param name="request">Role update</param>
    /// <returns>Success message</returns>
    [HttpPut("{orgId}/members/{userId}/role")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMemberRole(Guid orgId, Guid userId, [FromBody] object request)
    {
        var currentUserId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _organizationService.UpdateMemberRoleAsync(orgId, userId, Guid.Parse(currentUserId), request);
        return Ok(result);
    }

    /// <summary>
    /// Updates a member's status in an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="userId">User ID to update</param>
    /// <param name="request">Status update</param>
    /// <returns>Success message</returns>
    [HttpPut("{orgId}/members/{userId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMemberStatus(Guid orgId, Guid userId, [FromBody] UpdateMemberStatusRequest request)
    {
        var currentUserId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _organizationService.UpdateMemberStatusAsync(orgId, userId, Guid.Parse(currentUserId), request);
        return Ok(result);
    }

    /// <summary>
    /// Updates a member's rates in an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="userId">User ID to update</param>
    /// <param name="request">Rates update</param>
    /// <returns>Success message</returns>
    [HttpPut("{orgId}/members/{userId}/rates")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMemberRates(Guid orgId, Guid userId, [FromBody] object request)
    {
        var currentUserId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _organizationService.UpdateMemberRatesAsync(orgId, userId, Guid.Parse(currentUserId), request);
        return Ok(result);
    }

    /// <summary>
    /// Removes a member from an organization
    /// </summary>
    /// <param name="orgId">Organization ID</param>
    /// <param name="userId">User ID to remove</param>
    /// <returns>Success message</returns>
    [HttpDelete("{orgId}/members/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveMember(Guid orgId, Guid userId)
    {
        var currentUserId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var result = await _organizationService.RemoveMemberAsync(orgId, userId, Guid.Parse(currentUserId));
        return Ok(result);
    }
}
