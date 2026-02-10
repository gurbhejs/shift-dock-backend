using ShiftDock.Application.DTOs.Reports;

namespace ShiftDock.Application.Interfaces;

public interface IReportService
{
    Task<byte[]> GenerateWorkerReportAsync(Guid organizationId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateProjectReportAsync(Guid projectId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateAttendanceReportAsync(Guid organizationId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<object> GetAttendanceReportAsync(Guid organizationId, Guid currentUserId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<object> GetProjectReportAsync(Guid organizationId, Guid projectId, Guid currentUserId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectReportResponse>> GetProjectReportAsync(ProjectReportRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<object> GetPayrollReportAsync(PayrollReportRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}
