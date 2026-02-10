using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.Interfaces.Repositories;

public interface IJoinRequestRepository
{
    Task<JoinRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<JoinRequest>> GetByOrganizationIdAsync(string organizationId, JoinRequestStatus? status, CancellationToken cancellationToken = default);
    Task<IEnumerable<JoinRequest>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<JoinRequest?> GetPendingRequestAsync(string userId, string organizationId, CancellationToken cancellationToken = default);
    Task<int> GetPendingCountByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JoinRequest>> GetPendingByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<JoinRequest> AddAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default);
}
