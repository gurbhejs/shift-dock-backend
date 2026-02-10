using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.DTOs.Assignments;
using ShiftDock.Application.DTOs.Dashboard;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Enums;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly IJoinRequestRepository _joinRequestRepository;

    public DashboardService(
        ApplicationDbContext context,
        IOrganizationRepository organizationRepository,
        IProjectRepository projectRepository,
        IShiftRepository shiftRepository,
        IJoinRequestRepository joinRequestRepository)
    {
        _context = context;
        _organizationRepository = organizationRepository;
        _projectRepository = projectRepository;
        _shiftRepository = shiftRepository;
        _joinRequestRepository = joinRequestRepository;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        var totalMembers = await _context.OrganizationMemberships
            .Where(m => m.OrganizationId == organizationId.ToString())
            .CountAsync(cancellationToken);

        var totalProjects = await _projectRepository.GetCountByOrganizationIdAsync(organizationId.ToString(), cancellationToken);
        var activeProjects = await _projectRepository.GetActiveCountByOrganizationIdAsync(organizationId.ToString(), cancellationToken);

        var todayShifts = await _shiftRepository.GetTodayShiftsByOrganizationAsync(organizationId.ToString(), cancellationToken);
        var activeWorkers = await _context.OrganizationMemberships
            .Where(m => m.OrganizationId == organizationId.ToString() && m.Status == UserStatus.Active)
            .CountAsync(cancellationToken);

        return new DashboardStatsResponse
        {
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            TotalWorkers = totalMembers,
            ActiveWorkers = activeWorkers,
            TotalShifts = todayShifts.Count(),
            UpcomingShifts = todayShifts.Count(),
            CompletedShifts = 0, // TODO: Implement completed shifts count
            PendingAssignments = 0 // TODO: Implement pending assignments count
        };
    }

    public async Task<IEnumerable<TodayShiftResponse>> GetTodayShiftsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        var todayShifts = await _shiftRepository.GetTodayShiftsByOrganizationAsync(organizationId.ToString(), cancellationToken);

        var response = new List<TodayShiftResponse>();

        foreach (var shift in todayShifts)
        {
            var assignedWorkers = await _context.WorkerAssignments
                .Where(a => a.ShiftId == shift.Id)
                .Include(a => a.User)
                .ToListAsync(cancellationToken);

            response.Add(new TodayShiftResponse
            {
                ShiftId = shift.Id,
                ProjectName = shift.Project.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                AssignedWorkers = assignedWorkers.Count
            });
        }

        return response;
    }

    public async Task<IEnumerable<object>> GetPendingRequestsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var requests = await _joinRequestRepository.GetPendingByOrganizationAsync(organizationId.ToString(), cancellationToken);
        return requests.Select(r => new { r.Id, r.UserId, r.OrganizationId, r.Status, r.RequestedAt });
    }

    public async Task<WorkerDashboardResponse> GetWorkerDashboardAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.WorkerAssignments
            .Where(a => a.UserId == currentUserId.ToString())
            .Include(a => a.Shift)
                .ThenInclude(s => s.Project)
            .Where(a => a.Shift.Project.OrganizationId == organizationId.ToString())
            .ToListAsync(cancellationToken);

        var upcomingShifts = assignments
            .Where(a => a.Status == "Assigned" || a.Status == "Accepted")
            .Count();

        var completedShifts = assignments
            .Where(a => a.Status == "Completed")
            .Count();

        var pendingAssignments = assignments
            .Where(a => a.Status == "Assigned")
            .Count();

        var totalEarnings = 0m; // Calculate based on rates if needed

        return new WorkerDashboardResponse
        {
            UpcomingShifts = upcomingShifts,
            CompletedShifts = completedShifts,
            TotalEarnings = totalEarnings,
            PendingAssignments = pendingAssignments,
            RecentAssignments = new List<WorkerAssignmentResponse>()
        };
    }
}
