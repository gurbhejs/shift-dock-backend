using ShiftDock.Application.DTOs.Dashboard;

namespace ShiftDock.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodayShiftResponse>> GetTodayShiftsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetPendingRequestsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<WorkerDashboardResponse> GetWorkerDashboardAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
}
