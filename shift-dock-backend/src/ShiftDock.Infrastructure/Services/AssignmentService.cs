using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.DTOs.Assignments;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IShiftRepository shiftRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ApplicationDbContext context,
        INotificationService notificationService)
    {
        _assignmentRepository = assignmentRepository;
        _shiftRepository = shiftRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<WorkerAssignmentResponse>> GetAssignmentsByShiftAsync(Guid shiftId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.WorkerAssignments
            .Where(a => a.ShiftId == shiftId.ToString())
            .Include(a => a.User)
            .Include(a => a.Shift)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<WorkerAssignmentResponse>>(assignments);
    }

    public async Task<IEnumerable<WorkerAssignmentResponse>> GetAssignmentsByProjectAsync(Guid organizationId, Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Verify the project belongs to the organization and user has access
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId.ToString() && p.OrganizationId == organizationId.ToString(), cancellationToken);
        
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found in organization {organizationId}");
        }

        // Get all assignments for shifts in this project
        var assignments = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
            .ThenInclude(s => s.Project)
            .Where(a => a.Shift.ProjectId == projectId.ToString())
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<WorkerAssignmentResponse>>(assignments);
    }

    public async Task<IEnumerable<WorkerAssignmentResponse>> GetMyAssignmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.WorkerAssignments
            .Where(a => a.UserId == userId.ToString())
            .Include(a => a.User)
            .Include(a => a.Shift)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<WorkerAssignmentResponse>>(assignments);
    }

    public async Task<WorkerAssignmentResponse> AssignWorkerToShiftAsync(Guid shiftId, string workerId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId.ToString(), cancellationToken);
        if (shift == null)
        {
            throw new KeyNotFoundException($"Shift with ID {shiftId} not found");
        }

        var user = await _userRepository.GetByIdAsync(workerId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {workerId} not found");
        }

        // Check if already assigned
        var existingAssignment = await _context.WorkerAssignments
            .FirstOrDefaultAsync(a => a.ShiftId == shiftId.ToString() && a.UserId == workerId, cancellationToken);
        
        if (existingAssignment != null)
        {
            throw new InvalidOperationException("Worker is already assigned to this shift");
        }

        var assignment = new WorkerAssignment
        {
            ShiftId = shiftId.ToString(),
            UserId = workerId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to the user
        await _notificationService.SendNotificationAsync(
            Guid.Parse(workerId),
            "Shift Assigned",
            $"You have been assigned to a shift on {shift.ShiftDate} from {shift.StartTime} to {shift.EndTime}.",
            cancellationToken);

        // Reload with includes
        var createdAssignment = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
            .FirstOrDefaultAsync(a => a.Id == assignment.Id, cancellationToken);

        return _mapper.Map<WorkerAssignmentResponse>(createdAssignment);
    }

    public async Task<IEnumerable<WorkerAssignmentResponse>> BulkAssignWorkersAsync(Guid shiftId, List<string> workerIds, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId.ToString(), cancellationToken);
        if (shift == null)
        {
            throw new KeyNotFoundException($"Shift with ID {shiftId} not found");
        }

        var assignments = new List<WorkerAssignment>();

        foreach (var workerId in workerIds)
        {
            var user = await _userRepository.GetByIdAsync(workerId, cancellationToken);
            if (user == null)
            {
                continue; // Skip invalid user IDs
            }

            // Check if already assigned
            var existingAssignment = await _context.WorkerAssignments
                .FirstOrDefaultAsync(a => a.ShiftId == shiftId.ToString() && a.UserId == workerId, cancellationToken);
            
            if (existingAssignment != null)
            {
                continue; // Skip already assigned workers
            }

            var assignment = new WorkerAssignment
            {
                ShiftId = shiftId.ToString(),
                UserId = workerId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            assignments.Add(assignment);
        }

        if (assignments.Any())
        {
            foreach (var assignment in assignments)
            {
                await _assignmentRepository.AddAsync(assignment, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send notifications to all assigned workers
            var userIds = assignments.Select(a => Guid.Parse(a.UserId)).ToList();
            await _notificationService.SendBulkNotificationsAsync(
                userIds,
                "Shift Assigned",
                $"You have been assigned to a shift on {shift.ShiftDate} from {shift.StartTime} to {shift.EndTime}.",
                cancellationToken);
        }

        // Reload with includes
        var createdAssignments = await _context.WorkerAssignments
            .Where(a => a.ShiftId == shiftId.ToString())
            .Include(a => a.User)
            .Include(a => a.Shift)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<WorkerAssignmentResponse>>(createdAssignments);
    }

    public async Task<WorkerAssignmentResponse> UpdateAssignmentStatusAsync(Guid assignmentId, UpdateAssignmentStatusRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(assignmentId.ToString(), cancellationToken);
        if (assignment == null)
        {
            throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found");
        }

        assignment.Status = request.Status;
        
        if (request.ActualQuantity.HasValue)
        {
            assignment.ActualQuantity = request.ActualQuantity.Value;
        }

        if (!string.IsNullOrEmpty(request.Notes))
        {
            assignment.Notes = request.Notes;
        }

        assignment.UpdatedAt = DateTime.UtcNow;

        await _assignmentRepository.UpdateAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var updatedAssignment = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
            .FirstOrDefaultAsync(a => a.Id == assignment.Id, cancellationToken);

        return _mapper.Map<WorkerAssignmentResponse>(updatedAssignment);
    }

    public async Task<SyncProjectAssignmentsResponse> SyncProjectAssignmentsAsync(Guid organizationId, Guid projectId, SyncProjectAssignmentsRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Verify the project belongs to the organization
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId.ToString() && p.OrganizationId == organizationId.ToString(), cancellationToken);
        
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found in organization {organizationId}");
        }

        // Get all existing assignments for this project
        var existingAssignments = await _context.WorkerAssignments
            .Include(a => a.Shift)
            .Where(a => a.Shift.ProjectId == projectId.ToString())
            .ToListAsync(cancellationToken);

        int added = 0, updated = 0, deleted = 0;

        // Create lookup for new assignments
        var newAssignmentsLookup = request.Assignments
            .GroupBy(a => new { ShiftId = a.ShiftId, UserId = a.UserId })
            .ToDictionary(g => g.Key, g => g.First());

        // Track deleted assignments for notifications
        var deletedAssignments = new List<WorkerAssignment>();

        // Delete assignments not in the new list
        foreach (var existing in existingAssignments)
        {
            var key = new { ShiftId = existing.ShiftId, UserId = existing.UserId };
            if (!newAssignmentsLookup.ContainsKey(key))
            {
                deletedAssignments.Add(existing);
                await _assignmentRepository.DeleteAsync(existing, cancellationToken);
                deleted++;
            }
            else
            {
                // Update existing assignment if notes changed
                var newAssignment = newAssignmentsLookup[key];
                if (existing.Notes != newAssignment.Notes)
                {
                    existing.Notes = newAssignment.Notes;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _assignmentRepository.UpdateAsync(existing, cancellationToken);
                }
                updated++;
            }
        }

        // Add new assignments
        var existingAssignmentsLookup = existingAssignments
            .GroupBy(a => new { ShiftId = a.ShiftId, UserId = a.UserId })
            .ToDictionary(g => g.Key);

        // Track new assignments for notifications
        var newAssignmentsCreated = new List<(string UserId, string ShiftDate, string StartTime, string EndTime)>();

        foreach (var newAssignment in request.Assignments)
        {
            var key = new { ShiftId = newAssignment.ShiftId, UserId = newAssignment.UserId };
            if (!existingAssignmentsLookup.ContainsKey(key))
            {
                // Verify shift belongs to the project
                var shift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.Id == newAssignment.ShiftId && s.ProjectId == projectId.ToString(), cancellationToken);
                
                if (shift == null)
                {
                    continue; // Skip invalid shifts
                }

                // Verify user exists
                var user = await _userRepository.GetByIdAsync(newAssignment.UserId, cancellationToken);
                if (user == null)
                {
                    continue; // Skip invalid users
                }

                var assignment = new WorkerAssignment
                {
                    ShiftId = newAssignment.ShiftId,
                    UserId = newAssignment.UserId,
                    Status = "Pending",
                    Notes = newAssignment.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _assignmentRepository.AddAsync(assignment, cancellationToken);
                newAssignmentsCreated.Add((newAssignment.UserId, shift.ShiftDate, shift.StartTime, shift.EndTime));
                added++;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notifications for deleted assignments
        if (deletedAssignments.Any())
        {
            foreach (var deletedAssignment in deletedAssignments)
            {
                await _notificationService.SendNotificationAsync(
                    Guid.Parse(deletedAssignment.UserId),
                    "Shift Unassigned",
                    $"You have been unassigned from a shift on {deletedAssignment.Shift.ShiftDate} from {deletedAssignment.Shift.StartTime} to {deletedAssignment.Shift.EndTime}.",
                    cancellationToken);
            }
        }

        // Send notifications for new assignments
        if (newAssignmentsCreated.Any())
        {
            foreach (var (userId, shiftDate, startTime, endTime) in newAssignmentsCreated)
            {
                await _notificationService.SendNotificationAsync(
                    Guid.Parse(userId),
                    "Shift Assigned",
                    $"You have been assigned to a shift on {shiftDate} from {startTime} to {endTime}.",
                    cancellationToken);
            }
        }

        // Get current assignments after sync
        var currentAssignments = await _context.WorkerAssignments
            .Include(a => a.User)
            .Include(a => a.Shift)
            .ThenInclude(s => s.Project)
            .Where(a => a.Shift.ProjectId == projectId.ToString())
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return new SyncProjectAssignmentsResponse
        {
            Added = added,
            Updated = updated,
            Deleted = deleted,
            CurrentAssignments = _mapper.Map<List<WorkerAssignmentResponse>>(currentAssignments)
        };
    }

    public async Task<bool> RemoveAssignmentAsync(Guid assignmentId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.WorkerAssignments
            .Include(a => a.Shift)
            .FirstOrDefaultAsync(a => a.Id == assignmentId.ToString(), cancellationToken);
        
        if (assignment == null)
        {
            throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found");
        }

        // Store details before deletion for notification
        var userId = assignment.UserId;
        var shiftDate = assignment.Shift.ShiftDate;
        var startTime = assignment.Shift.StartTime;
        var endTime = assignment.Shift.EndTime;

        await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to the user
        await _notificationService.SendNotificationAsync(
            Guid.Parse(userId),
            "Shift Unassigned",
            $"You have been unassigned from a shift on {shiftDate} from {startTime} to {endTime}.",
            cancellationToken);

        return true;
    }

    public async Task UnassignFutureProjectShiftsAsync(string projectId, string projectName, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Get all future shifts for this project
        var futureShifts = await _context.Shifts
            .Where(s => s.ProjectId == projectId && string.Compare(s.ShiftDate, today) >= 0)
            .ToListAsync(cancellationToken);

        if (!futureShifts.Any())
        {
            return;
        }

        var shiftIds = futureShifts.Select(s => s.Id).ToList();

        // Get all assignments for future shifts
        var assignments = await _context.WorkerAssignments
            .Include(a => a.Shift)
            .Where(a => shiftIds.Contains(a.ShiftId))
            .ToListAsync(cancellationToken);

        if (!assignments.Any())
        {
            return;
        }

        // Group assignments by user for batch notification
        var userAssignments = assignments.GroupBy(a => a.UserId).ToList();

        // Delete all assignments using the repository pattern
        foreach (var assignment in assignments)
        {
            await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notifications to each affected user
        foreach (var userGroup in userAssignments)
        {
            var userId = userGroup.Key;
            var assignmentCount = userGroup.Count();
            var shiftDates = userGroup.Select(a => a.Shift.ShiftDate).Distinct().OrderBy(d => d).ToList();

            var message = assignmentCount == 1
                ? $"Your shift assignment for project '{projectName}' on {shiftDates.First()} has been cancelled due to project status change."
                : $"Your {assignmentCount} shift assignments for project '{projectName}' have been cancelled due to project status change.";

            await _notificationService.SendNotificationAsync(
                Guid.Parse(userId),
                "Shifts Cancelled",
                message,
                cancellationToken);
        }
    }
}
