using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Organizations;

namespace ShiftDock.Application.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationResponse> CreateOrganizationAsync(Guid userId, CreateOrganizationRequest request, CancellationToken cancellationToken = default);
    Task<OrganizationResponse> GetOrganizationByIdAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationResponse>> GetUserOrganizationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrganizationResponse> UpdateOrganizationAsync(Guid organizationId, Guid currentUserId, UpdateOrganizationRequest request, CancellationToken cancellationToken = default);
    Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> JoinOrganizationAsync(Guid userId, JoinOrganizationRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<JoinRequestResponse>> GetJoinRequestsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<HandleJoinRequestResponse> HandleJoinRequestsAsync(Guid organizationId, Guid currentUserId, HandleJoinRequestRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetMembersAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<object> UpdateMemberRoleAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default);
    Task<object> UpdateMemberStatusAsync(Guid organizationId, Guid userId, Guid currentUserId, UpdateMemberStatusRequest request, CancellationToken cancellationToken = default);
    Task<object> UpdateMemberRatesAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default);
    Task<object> RemoveMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, CancellationToken cancellationToken = default);
}
