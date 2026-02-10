using ShiftDock.Domain.Entities;

namespace ShiftDock.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByOrganizationIdAsync(string organizationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetCountByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<int> GetActiveCountByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Project project, CancellationToken cancellationToken = default);
}
