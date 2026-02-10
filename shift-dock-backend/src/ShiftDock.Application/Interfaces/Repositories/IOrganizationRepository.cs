using ShiftDock.Domain.Entities;

namespace ShiftDock.Application.Interfaces.Repositories;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Organization>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);
    Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
