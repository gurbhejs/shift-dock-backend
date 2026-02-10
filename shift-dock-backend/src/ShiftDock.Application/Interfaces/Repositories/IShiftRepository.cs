using ShiftDock.Domain.Entities;

namespace ShiftDock.Application.Interfaces.Repositories;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shift>> GetByProjectIdAsync(string projectId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shift>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shift>> GetTodayShiftsByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<Shift> AddAsync(Shift shift, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Shift> shifts, CancellationToken cancellationToken = default);
    Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default);
    Task DeleteAsync(Shift shift, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<Shift> shifts, CancellationToken cancellationToken = default);
}
