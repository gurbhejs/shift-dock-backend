using ShiftDock.Domain.Entities;

namespace ShiftDock.Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    Task<WorkerAssignment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkerAssignment>> GetByProjectIdAsync(string projectId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetCountByProjectIdAsync(string projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkerAssignment>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<WorkerAssignment?> GetByProjectAndUserAsync(string projectId, string userId, CancellationToken cancellationToken = default);
    Task<WorkerAssignment> AddAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default);
}
