using AutoMapper;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Projects;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;

namespace ShiftDock.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly IAssignmentService _assignmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IOrganizationRepository organizationRepository,
        IShiftRepository shiftRepository,
        IAssignmentService assignmentService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _organizationRepository = organizationRepository;
        _shiftRepository = shiftRepository;
        _assignmentService = assignmentService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectResponse> CreateProjectAsync(Guid organizationId, Guid currentUserId, CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        var project = new Project
        {
            OrganizationId = organizationId.ToString(),
            Name = request.Name,
            Location = request.Location,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes,
            WorkType = request.WorkType,
            Rate = request.Rate,
            ContractStatus = request.ContractStatus,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> GetProjectByIdAsync(Guid projectId, Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<IEnumerable<ProjectResponse>> GetProjectsByOrganizationAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByOrganizationIdAsync(organizationId.ToString(), 1, 1000, cancellationToken);
        return _mapper.Map<List<ProjectResponse>>(projects);
    }

    public async Task<ProjectResponse> UpdateProjectAsync(Guid projectId, Guid organizationId, Guid currentUserId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        // Track original status to detect changes
        var originalStatus = project.ContractStatus;

        if (!string.IsNullOrEmpty(request.Name))
        {
            project.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            project.Location = request.Location;
        }

        if (request.Latitude != null)
        {
            project.Latitude = request.Latitude;
        }

        if (request.Longitude != null)
        {
            project.Longitude = request.Longitude;
        }

        if (request.Notes != null)
        {
            project.Notes = request.Notes;
        }

        if (request.WorkType.HasValue)
        {
            project.WorkType = request.WorkType.Value;
        }

        if (request.Rate.HasValue)
        {
            project.Rate = request.Rate.Value;
        }

        if (request.ContractStatus.HasValue)
        {
            project.ContractStatus = request.ContractStatus.Value;
        }

        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Check if status changed from Active to inactive (Paused, Expired, or Cancelled)
        if (originalStatus == ContractStatus.Active && 
            request.ContractStatus.HasValue && 
            (request.ContractStatus.Value == ContractStatus.Paused || 
             request.ContractStatus.Value == ContractStatus.Expired || 
             request.ContractStatus.Value == ContractStatus.Cancelled))
        {
            // Unassign all future shifts and notify users via AssignmentService
            await _assignmentService.UnassignFutureProjectShiftsAsync(projectId.ToString(), project.Name, cancellationToken);
        }

        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        await _projectRepository.DeleteAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ShiftDto>> GetProjectShiftsAsync(Guid projectId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        var shifts = await _shiftRepository.GetByProjectIdAsync(projectId.ToString(), startDate, endDate, cancellationToken);
        return _mapper.Map<IEnumerable<ShiftDto>>(shifts);
    }

    public async Task<IEnumerable<object>> GetAddressSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        // TODO: Implement address autocomplete using a geocoding API
        await Task.CompletedTask;
        return new List<object>();
    }

    public async Task<ProjectResponse> CreateProjectAsync(string organizationId, CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        return await CreateProjectAsync(Guid.Parse(organizationId), Guid.Empty, request, cancellationToken);
    }

    public async Task<ProjectResponse> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default)
    {
        return await GetProjectByIdAsync(Guid.Parse(projectId), Guid.Empty, Guid.Empty, cancellationToken);
    }

    public async Task<PaginatedResponse<ProjectResponse>> GetOrganizationProjectsAsync(string organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByOrganizationIdAsync(organizationId, page, pageSize, cancellationToken);
        var totalCount = await _projectRepository.GetCountByOrganizationIdAsync(organizationId, cancellationToken);

        var projectResponses = _mapper.Map<List<ProjectResponse>>(projects);

        return new PaginatedResponse<ProjectResponse>
        {
            Items = projectResponses,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ProjectResponse> UpdateProjectAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        // Get the project first to access its OrganizationId
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }
        
        return await UpdateProjectAsync(Guid.Parse(projectId), Guid.Parse(project.OrganizationId), Guid.Empty, request, cancellationToken);
    }

    public async Task DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
    {
        await DeleteProjectAsync(Guid.Parse(projectId), Guid.Empty, Guid.Empty, cancellationToken);
    }

    public async Task<IEnumerable<ShiftDto>> GetProjectShiftsAsync(string projectId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        return await GetProjectShiftsAsync(Guid.Parse(projectId), startDate, endDate, cancellationToken);
    }

    public async Task<ProjectResponse> GetProjectByIdAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> UpdateProjectAsync(Guid projectId, Guid currentUserId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        // Use the main UpdateProjectAsync method that handles status changes
        return await UpdateProjectAsync(projectId, Guid.Parse(project.OrganizationId), currentUserId, request, cancellationToken);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        await _projectRepository.DeleteAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ShiftDto>> GetShiftsByProjectAsync(Guid projectId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var shifts = await _shiftRepository.GetByProjectIdAsync(projectId.ToString(), null, null, cancellationToken);
        return _mapper.Map<List<ShiftDto>>(shifts);
    }

    public async Task<ShiftDto> CreateShiftAsync(Guid projectId, Guid currentUserId, CreateShiftRequest request, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        var shift = new Shift
        {
            ProjectId = projectId.ToString(),
            ShiftDate = request.ShiftDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TargetQuantity = request.TargetQuantity,
            CreatedAt = DateTime.UtcNow
        };

        await _shiftRepository.AddAsync(shift, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ShiftDto>(shift);
    }

    public async Task<ShiftDto> UpdateShiftAsync(Guid projectId, Guid shiftId, Guid currentUserId, UpdateShiftRequest request, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId.ToString(), cancellationToken);
        if (shift == null)
        {
            throw new KeyNotFoundException($"Shift with ID {shiftId} not found");
        }

        if (!string.IsNullOrEmpty(request.ShiftDate))
        {
            shift.ShiftDate = request.ShiftDate;
        }

        if (!string.IsNullOrEmpty(request.StartTime))
        {
            shift.StartTime = request.StartTime;
        }

        if (!string.IsNullOrEmpty(request.EndTime))
        {
            shift.EndTime = request.EndTime;
        }

        if (request.TargetQuantity.HasValue)
        {
            shift.TargetQuantity = request.TargetQuantity;
        }

        await _shiftRepository.UpdateAsync(shift, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ShiftDto>(shift);
    }

    public async Task<bool> DeleteShiftAsync(Guid projectId, Guid shiftId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId.ToString(), cancellationToken);
        if (shift == null)
        {
            throw new KeyNotFoundException($"Shift with ID {shiftId} not found");
        }

        await _shiftRepository.DeleteAsync(shift, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ShiftDto>> SyncShiftsAsync(Guid projectId, Guid currentUserId, SyncShiftsRequest request, CancellationToken cancellationToken = default)
    {
        // Verify project exists
        var project = await _projectRepository.GetByIdAsync(projectId.ToString(), cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        // Get all existing shifts for this project
        var existingShifts = await _shiftRepository.GetByProjectIdAsync(projectId.ToString(), null, null, cancellationToken);
        var existingShiftsDict = existingShifts.ToDictionary(s => s.Id);

        // Track shifts to add, update, and delete
        var shiftsToUpdate = new List<Shift>();
        var shiftsToAdd = new List<Shift>();
        var processedIds = new HashSet<string>();

        // Process incoming shifts
        foreach (var shiftItem in request.Shifts)
        {
            if (!string.IsNullOrEmpty(shiftItem.Id))
            {
                // Existing shift - update it
                if (existingShiftsDict.TryGetValue(shiftItem.Id, out var existingShift))
                {
                    existingShift.ShiftDate = shiftItem.ShiftDate;
                    existingShift.StartTime = shiftItem.StartTime;
                    existingShift.EndTime = shiftItem.EndTime;
                    existingShift.TargetQuantity = shiftItem.TargetQuantity;
                    shiftsToUpdate.Add(existingShift);
                    processedIds.Add(shiftItem.Id);
                }
            }
            else
            {
                // New shift - add it
                var newShift = new Shift
                {
                    ProjectId = projectId.ToString(),
                    ShiftDate = shiftItem.ShiftDate,
                    StartTime = shiftItem.StartTime,
                    EndTime = shiftItem.EndTime,
                    TargetQuantity = shiftItem.TargetQuantity,
                    CreatedAt = DateTime.UtcNow
                };
                shiftsToAdd.Add(newShift);
            }
        }

        // Find shifts to delete (existing but not in the incoming list)
        var shiftsToDelete = existingShiftsDict.Values
            .Where(s => !processedIds.Contains(s.Id))
            .ToList();

        // Execute database operations
        if (shiftsToDelete.Any())
        {
            await _shiftRepository.DeleteRangeAsync(shiftsToDelete, cancellationToken);
        }

        if (shiftsToAdd.Any())
        {
            await _shiftRepository.AddRangeAsync(shiftsToAdd, cancellationToken);
        }

        foreach (var shift in shiftsToUpdate)
        {
            await _shiftRepository.UpdateAsync(shift, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return all shifts for this project
        var allShifts = await _shiftRepository.GetByProjectIdAsync(projectId.ToString(), null, null, cancellationToken);
        return _mapper.Map<IEnumerable<ShiftDto>>(allShifts);
    }
}
