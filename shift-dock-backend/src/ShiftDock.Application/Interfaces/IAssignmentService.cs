using ShiftDock.Application.DTOs.Assignments;
using ShiftDock.Application.DTOs.Common;

namespace ShiftDock.Application.Interfaces;

public interface IAssignmentService
{
    Task<IEnumerable<WorkerAssignmentResponse>> GetAssignmentsByShiftAsync(Guid shiftId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkerAssignmentResponse>> GetMyAssignmentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkerAssignmentResponse> AssignWorkerToShiftAsync(Guid shiftId, string workerId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkerAssignmentResponse>> BulkAssignWorkersAsync(Guid shiftId, List<string> workerIds, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<WorkerAssignmentResponse> UpdateAssignmentStatusAsync(Guid assignmentId, UpdateAssignmentStatusRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<bool> RemoveAssignmentAsync(Guid assignmentId, Guid currentUserId, CancellationToken cancellationToken = default);
}
