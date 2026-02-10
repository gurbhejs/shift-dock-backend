using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _context;

    public ShiftRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shift?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .Include(s => s.Project)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetByProjectIdAsync(string projectId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Shifts.Where(s => s.ProjectId == projectId);

        if (startDate.HasValue)
        {
            var startDateStr = startDate.Value.ToString("yyyy-MM-dd");
            query = query.Where(s => string.Compare(s.ShiftDate, startDateStr) >= 0);
        }

        if (endDate.HasValue)
        {
            var endDateStr = endDate.Value.ToString("yyyy-MM-dd");
            query = query.Where(s => string.Compare(s.ShiftDate, endDateStr) <= 0);
        }

        return await query
            .OrderBy(s => s.ShiftDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        return await _context.Shifts
            .Include(s => s.Project)
            .Where(s => s.ShiftDate == dateStr)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetTodayShiftsByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        var todayStr = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        return await _context.Shifts
            .Include(s => s.Project)
            .Where(s => s.Project.OrganizationId == organizationId && s.ShiftDate == todayStr)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shift> AddAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        await _context.Shifts.AddAsync(shift, cancellationToken);
        return shift;
    }

    public async Task AddRangeAsync(IEnumerable<Shift> shifts, CancellationToken cancellationToken = default)
    {
        await _context.Shifts.AddRangeAsync(shifts, cancellationToken);
    }

    public Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Update(shift);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Remove(shift);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<Shift> shifts, CancellationToken cancellationToken = default)
    {
        _context.Shifts.RemoveRange(shifts);
        return Task.CompletedTask;
    }
}
