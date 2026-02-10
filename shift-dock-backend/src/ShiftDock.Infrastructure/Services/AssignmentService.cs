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

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IShiftRepository shiftRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _assignmentRepository = assignmentRepository;
        _shiftRepository = shiftRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
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

    public async Task<bool> RemoveAssignmentAsync(Guid assignmentId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(assignmentId.ToString(), cancellationToken);
        if (assignment == null)
        {
            throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found");
        }

        await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
