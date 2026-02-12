using System.Text;
using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.DTOs.Reports;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IProjectRepository _projectRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public ReportService(
        ApplicationDbContext context,
        IProjectRepository projectRepository,
        IOrganizationRepository organizationRepository)
    {
        _context = context;
        _projectRepository = projectRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<byte[]> GenerateWorkerReportAsync(Guid organizationId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        var assignments = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
                .ThenInclude(s => s.Project)
            .Where(a => a.Shift.Project.OrganizationId == organizationId.ToString())
            .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Worker Name,Phone,Shift Date,Project,Status,Actual Quantity,Assigned Date");

        foreach (var assignment in assignments)
        {
            var projectName = assignment.Shift?.Project?.Name ?? "N/A";
            csv.AppendLine($"{assignment.User.Name},{assignment.User.Phone},{assignment.Shift?.ShiftDate},{projectName},{assignment.Status},{assignment.ActualQuantity},{assignment.CreatedAt:yyyy-MM-dd}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> GenerateProjectReportAsync(Guid projectId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        var startDateStr = startDate.ToString("yyyy-MM-dd");
        var endDateStr = endDate.ToString("yyyy-MM-dd");

        var shifts = await _context.Shifts
            .Include(s => s.Assignments)
                .ThenInclude(a => a.User)
            .Where(s => s.ProjectId == projectId.ToString())
            .Where(s => string.Compare(s.ShiftDate, startDateStr) >= 0 && string.Compare(s.ShiftDate, endDateStr) <= 0)
            .OrderBy(s => s.ShiftDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine($"Project Report: {project.Name}");
        csv.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        csv.AppendLine();
        csv.AppendLine("Date,Start Time,End Time,Target Quantity,Assigned Workers");

        foreach (var shift in shifts)
        {
            var workerCount = shift.Assignments?.Count ?? 0;
            csv.AppendLine($"{shift.ShiftDate},{shift.StartTime},{shift.EndTime},{shift.TargetQuantity},{workerCount}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> GenerateAttendanceReportAsync(Guid organizationId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        var startDateStr = startDate.ToString("yyyy-MM-dd");
        var endDateStr = endDate.ToString("yyyy-MM-dd");

        var shifts = await _context.Shifts
            .Include(s => s.Project)
            .Include(s => s.Assignments)
            .Where(s => s.Project.OrganizationId == organizationId.ToString())
            .Where(s => string.Compare(s.ShiftDate, startDateStr) >= 0 && string.Compare(s.ShiftDate, endDateStr) <= 0)
            .OrderBy(s => s.ShiftDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine($"Attendance Report: {organization.Name}");
        csv.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        csv.AppendLine();
        csv.AppendLine("Date,Project,Start Time,End Time,Target Quantity,Total Workers");

        foreach (var shift in shifts)
        {
            var totalWorkers = shift.Assignments?.Count ?? 0;
            csv.AppendLine($"{shift.ShiftDate},{shift.Project.Name},{shift.StartTime},{shift.EndTime},{shift.TargetQuantity},{totalWorkers}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<object> GetAttendanceReportAsync(Guid organizationId, Guid currentUserId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        // Implement report logic here
        return await Task.FromResult(new { Message = "Attendance report" });
    }

    public async Task<object> GetProjectReportAsync(Guid organizationId, Guid projectId, Guid currentUserId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        // Implement report logic here
        return await Task.FromResult(new { Message = "Project report" });
    }

    public async Task<IEnumerable<ProjectReportResponse>> GetProjectReportAsync(ProjectReportRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var projects = await _context.Projects
            .Where(p => p.OrganizationId == request.OrganizationId)
            .Include(p => p.Shifts)
            .ToListAsync(cancellationToken);

        var reportItems = new List<ProjectReportResponse>();

        foreach (var project in projects)
        {
            var totalShifts = project.Shifts?.Count ?? 0;
            var completedShifts = project.Shifts?.Count(s => s.Assignments != null && s.Assignments.Any(a => a.Status == "Completed")) ?? 0;

            reportItems.Add(new ProjectReportResponse
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                TotalShifts = totalShifts,
                CompletedShifts = completedShifts,
                TotalWorkers = 0, // Calculate if needed
                TotalCost = 0m // Calculate if needed
            });
        }

        return reportItems;
    }

    public async Task<object> GetPayrollReportAsync(PayrollReportRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
                .ThenInclude(s => s.Project)
            .Where(a => a.Shift.Project.OrganizationId == request.OrganizationId)
            .Where(a => a.Status == "Completed")
            .ToListAsync(cancellationToken);

        var payrollData = assignments
            .GroupBy(a => a.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                UserName = g.First().User.Name,
                TotalShifts = g.Count(),
                TotalQuantity = g.Sum(a => a.ActualQuantity ?? 0),
                // Calculate earnings based on organization rates
                TotalEarnings = 0m
            });

        return new
        {
            OrganizationId = request.OrganizationId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PayrollData = payrollData
        };
    }
}
