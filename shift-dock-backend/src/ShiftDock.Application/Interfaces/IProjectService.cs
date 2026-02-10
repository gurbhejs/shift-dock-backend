using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Projects;

namespace ShiftDock.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateProjectAsync(Guid organizationId, Guid currentUserId, CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<ProjectResponse> GetProjectByIdAsync(Guid projectId, Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<ProjectResponse> GetProjectByIdAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectResponse>> GetProjectsByOrganizationAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<ProjectResponse> UpdateProjectAsync(Guid projectId, Guid organizationId, Guid currentUserId, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task<ProjectResponse> UpdateProjectAsync(Guid projectId, Guid currentUserId, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(Guid projectId, Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftDto>> GetProjectShiftsAsync(Guid projectId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftDto>> GetShiftsByProjectAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<ShiftDto> CreateShiftAsync(Guid projectId, Guid currentUserId, CreateShiftRequest request, CancellationToken cancellationToken = default);
    Task<ShiftDto> UpdateShiftAsync(Guid projectId, Guid shiftId, Guid currentUserId, UpdateShiftRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteShiftAsync(Guid projectId, Guid shiftId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftDto>> SyncShiftsAsync(Guid projectId, Guid currentUserId, SyncShiftsRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetAddressSuggestionsAsync(string query, CancellationToken cancellationToken = default);
}
