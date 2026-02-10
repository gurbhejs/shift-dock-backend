using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Organization)
            .Include(p => p.Shifts)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByOrganizationIdAsync(string organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.OrganizationId == organizationId)
            .Include(p => p.Shifts)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.OrganizationId == organizationId)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.OrganizationId == organizationId && p.Status == "Active")
            .CountAsync(cancellationToken);
    }

    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        return project;
    }

    public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.Projects.Update(project);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Remove(project);
        return Task.CompletedTask;
    }
}
