using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .Include(o => o.Memberships)
            .Include(o => o.Projects)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(o => o.JoinCode == code, cancellationToken);
    }

    public async Task<IEnumerable<Organization>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizationMemberships
            .Where(m => m.UserId == userId)
            .Include(m => m.Organization)
            .Select(m => m.Organization)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizationMemberships
            .Where(m => m.UserId == userId)
            .CountAsync(cancellationToken);
    }

    public async Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _context.Organizations.AddAsync(organization, cancellationToken);
        return organization;
    }

    public Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        organization.UpdatedAt = DateTime.UtcNow;
        _context.Organizations.Update(organization);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Remove(organization);
        return Task.CompletedTask;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations.AnyAsync(o => o.JoinCode == code, cancellationToken);
    }
}
