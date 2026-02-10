using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkerAssignment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .Include(a => a.Shift)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WorkerAssignment>> GetByProjectIdAsync(string projectId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .Where(a => a.ShiftId == projectId)
            .Include(a => a.User)
            .Include(a => a.Shift)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByProjectIdAsync(string projectId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .Where(a => a.ShiftId == projectId)
            .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkerAssignment>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .Where(a => a.UserId == userId)
            .Include(a => a.Shift)
            .Include(a => a.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .Where(a => a.UserId == userId)
            .CountAsync(cancellationToken);
    }

    public async Task<WorkerAssignment?> GetByProjectAndUserAsync(string projectId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkerAssignments
            .FirstOrDefaultAsync(a => a.ShiftId == projectId && a.UserId == userId, cancellationToken);
    }

    public async Task<WorkerAssignment> AddAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.WorkerAssignments.AddAsync(assignment, cancellationToken);
        return assignment;
    }

    public Task UpdateAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default)
    {
        assignment.UpdatedAt = DateTime.UtcNow;
        _context.WorkerAssignments.Update(assignment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(WorkerAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.WorkerAssignments.Remove(assignment);
        return Task.CompletedTask;
    }
}
