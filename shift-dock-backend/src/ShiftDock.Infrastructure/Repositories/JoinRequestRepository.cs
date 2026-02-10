using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class JoinRequestRepository : IJoinRequestRepository
{
    private readonly ApplicationDbContext _context;

    public JoinRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JoinRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.JoinRequests
            .Include(j => j.Organization)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<JoinRequest>> GetByOrganizationIdAsync(string organizationId, JoinRequestStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.JoinRequests
            .Where(j => j.OrganizationId == organizationId);

        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        return await query
            .Include(j => j.Organization)
            .Include(j => j.User)
            .OrderByDescending(j => j.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<JoinRequest>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.JoinRequests
            .Where(j => j.UserId == userId)
            .Include(j => j.Organization)
            .OrderByDescending(j => j.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<JoinRequest?> GetPendingRequestAsync(string userId, string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.JoinRequests
            .FirstOrDefaultAsync(j => j.UserId == userId && 
                                     j.OrganizationId == organizationId && 
                                     j.Status == JoinRequestStatus.Pending, 
                                cancellationToken);
    }

    public async Task<int> GetPendingCountByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.JoinRequests
            .Where(j => j.OrganizationId == organizationId && j.Status == JoinRequestStatus.Pending)
            .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<JoinRequest>> GetPendingByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.JoinRequests
            .Where(j => j.OrganizationId == organizationId && j.Status == JoinRequestStatus.Pending)
            .Include(j => j.User)
            .OrderByDescending(j => j.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<JoinRequest> AddAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default)
    {
        await _context.JoinRequests.AddAsync(joinRequest, cancellationToken);
        return joinRequest;
    }

    public Task UpdateAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default)
    {
        _context.JoinRequests.Update(joinRequest);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(JoinRequest joinRequest, CancellationToken cancellationToken = default)
    {
        _context.JoinRequests.Remove(joinRequest);
        return Task.CompletedTask;
    }
}
