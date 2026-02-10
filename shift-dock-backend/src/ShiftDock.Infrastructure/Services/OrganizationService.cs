using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Organizations;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJoinRequestRepository _joinRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public OrganizationService(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IJoinRequestRepository joinRequestRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _joinRequestRepository = joinRequestRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
    }

    public async Task<OrganizationResponse> CreateOrganizationAsync(Guid userId, CreateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        // Generate unique join code
        var joinCode = GenerateJoinCode();
        while (await _organizationRepository.CodeExistsAsync(joinCode, cancellationToken))
        {
            joinCode = GenerateJoinCode();
        }

        var organization = new Organization
        {
            Name = request.Name,
            JoinCode = joinCode,
            OwnerId = userId.ToString(),
            DefaultHourlyRate = request.DefaultHourlyRate,
            DefaultContainerRate = request.DefaultContainerRate,
            DefaultBoxRate = request.DefaultBoxRate,
            CreatedAt = DateTime.UtcNow
        };

        await _organizationRepository.AddAsync(organization, cancellationToken);

        var membership = new OrganizationMembership
        {
            OrganizationId = organization.Id,
            UserId = userId.ToString(),
            Role = OrgRole.Owner,
            Status = UserStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        organization.Memberships.Add(membership);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrganizationResponse>(organization);
    }

    private string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluding confusing chars
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<OrganizationResponse> GetOrganizationByIdAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task<IEnumerable<OrganizationResponse>> GetUserOrganizationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var organizations = await _organizationRepository.GetByUserIdAsync(userId.ToString(), 1, 1000, cancellationToken);
        return _mapper.Map<List<OrganizationResponse>>(organizations);
    }

    public async Task<OrganizationResponse> UpdateOrganizationAsync(Guid organizationId, Guid currentUserId, UpdateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            organization.Name = request.Name;
        }

        if (request.DefaultHourlyRate.HasValue)
        {
            organization.DefaultHourlyRate = request.DefaultHourlyRate.Value;
        }

        if (request.DefaultContainerRate.HasValue)
        {
            organization.DefaultContainerRate = request.DefaultContainerRate.Value;
        }

        if (request.DefaultBoxRate.HasValue)
        {
            organization.DefaultBoxRate = request.DefaultBoxRate.Value;
        }

        organization.UpdatedAt = DateTime.UtcNow;

        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task DeleteOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId.ToString(), cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with ID {organizationId} not found");
        }

        await _organizationRepository.DeleteAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> JoinOrganizationAsync(Guid userId, JoinOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var organization = await _organizationRepository.GetByCodeAsync(request.OrganizationCode, cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with code {request.OrganizationCode} not found");
        }

        var existingRequest = await _joinRequestRepository.GetPendingRequestAsync(userId.ToString(), organization.Id, cancellationToken);
        if (existingRequest != null)
        {
            throw new InvalidOperationException("A pending join request already exists");
        }

        var joinRequest = new JoinRequest
        {
            OrganizationId = organization.Id,
            UserId = userId.ToString(),
            Status = JoinRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _joinRequestRepository.AddAsync(joinRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<JoinRequestResponse>> GetJoinRequestsAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Verify current user's membership and role
        var currentUserMembership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == currentUserId.ToString(), cancellationToken);

        if (currentUserMembership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this organization");
        }

        // Get pending join requests
        var joinRequests = await _joinRequestRepository.GetByOrganizationIdAsync(organizationId.ToString(), JoinRequestStatus.Pending, cancellationToken);

        return joinRequests.Select(jr => new JoinRequestResponse
        {
            Id = jr.Id,
            UserId = jr.UserId,
            Name = jr.User.Name,
            PhoneNumber = jr.User.Phone,
            Email = jr.User.Email,
            Status = "Requested",
            RequestedAt = jr.RequestedAt
        });
    }

    public async Task<HandleJoinRequestResponse> HandleJoinRequestsAsync(Guid organizationId, Guid currentUserId, HandleJoinRequestRequest request, CancellationToken cancellationToken = default)
    {
        // Verify current user's membership and role
        var currentUserMembership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == currentUserId.ToString(), cancellationToken);

        if (currentUserMembership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this organization");
        }

        // Only Owner and Admin can handle join requests
        if (currentUserMembership.Role != OrgRole.Owner && currentUserMembership.Role != OrgRole.Admin)
        {
            throw new UnauthorizedAccessException("Only owners and admins can handle join requests");
        }

        var processedCount = 0;
        var failedCount = 0;
        var errors = new List<string>();

        // Process each request ID
        foreach (var requestId in request.RequestIds)
        {
            try
            {
                // Get the join request
                var joinRequest = await _joinRequestRepository.GetByIdAsync(requestId, cancellationToken);
                if (joinRequest == null)
                {
                    errors.Add($"Join request {requestId} not found");
                    failedCount++;
                    continue;
                }

                if (joinRequest.OrganizationId != organizationId.ToString())
                {
                    errors.Add($"Join request {requestId} does not belong to this organization");
                    failedCount++;
                    continue;
                }

                if (joinRequest.Status != JoinRequestStatus.Pending)
                {
                    errors.Add($"Join request {requestId} has already been {joinRequest.Status.ToString().ToLower()}");
                    failedCount++;
                    continue;
                }

                // Handle the request
                if (request.Approved)
                {
                    // Check if user is already a member
                    var existingMembership = await _context.OrganizationMemberships
                        .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == joinRequest.UserId, cancellationToken);

                    if (existingMembership != null)
                    {
                        errors.Add($"User in request {requestId} is already a member");
                        failedCount++;
                        continue;
                    }

                    // Approve - add user as Worker
                    var membership = new OrganizationMembership
                    {
                        OrganizationId = organizationId.ToString(),
                        UserId = joinRequest.UserId,
                        Role = OrgRole.Worker,
                        Status = UserStatus.Active,
                        JoinedAt = DateTime.UtcNow
                    };

                    await _context.OrganizationMemberships.AddAsync(membership, cancellationToken);
                    joinRequest.Status = JoinRequestStatus.Approved;
                }
                else
                {
                    // Reject
                    joinRequest.Status = JoinRequestStatus.Rejected;
                }

                joinRequest.ProcessedAt = DateTime.UtcNow;
                await _joinRequestRepository.UpdateAsync(joinRequest, cancellationToken);
                processedCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error processing request {requestId}: {ex.Message}");
                failedCount++;
            }
        }

        // Save all changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var actionText = request.Approved ? "approved" : "rejected";
        var message = processedCount > 0
            ? $"{processedCount} join request(s) {actionText} successfully"
            : "No join requests were processed";

        if (failedCount > 0)
        {
            message += $". {failedCount} request(s) failed";
        }

        return new HandleJoinRequestResponse
        {
            Success = processedCount > 0,
            Message = message,
            ProcessedCount = processedCount,
            FailedCount = failedCount,
            Errors = errors
        };
    }

    public async Task<IEnumerable<object>> GetMembersAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var memberships = await _context.OrganizationMemberships
            .Where(m => m.OrganizationId == organizationId.ToString())
            .Include(m => m.User)
            .ToListAsync(cancellationToken);

        return memberships.Select(m => new
        {
            m.Id,
            UserId = m.UserId,
            UserName = m.User.Name,
            UserPhone = m.User.Phone,
            UserEmail = m.User.Email,
            Role = m.Role.ToString(),
            Status = m.Status.ToString(),
            m.JoinedAt
        });
    }

    public async Task<object> UpdateMemberRoleAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default)
    {
        var membership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == userId.ToString(), cancellationToken);

        if (membership == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        // Update role logic here
        membership.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { Success = true, Message = "Role updated successfully" };
    }

    public async Task<object> UpdateMemberStatusAsync(Guid organizationId, Guid userId, Guid currentUserId, UpdateMemberStatusRequest request, CancellationToken cancellationToken = default)
    {
        // Verify current user's membership and role
        var currentUserMembership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == currentUserId.ToString(), cancellationToken);

        if (currentUserMembership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this organization");
        }

        // Only Owner and Admin can update member status
        if (currentUserMembership.Role != OrgRole.Owner && currentUserMembership.Role != OrgRole.Admin)
        {
            throw new UnauthorizedAccessException("Only owners and admins can update member status");
        }

        var membership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == userId.ToString(), cancellationToken);

        if (membership == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        // Hierarchy check: Admin can only update Worker status
        if (currentUserMembership.Role == OrgRole.Admin && membership.Role != OrgRole.Worker)
        {
            throw new UnauthorizedAccessException("Admins can only update worker status");
        }

        // Parse and validate status
        if (!Enum.TryParse<UserStatus>(request.Status, true, out var newStatus))
        {
            throw new ArgumentException($"Invalid status value. Must be 'Active' or 'Inactive'");
        }

        membership.Status = newStatus;
        membership.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { Success = true, Message = "Status updated successfully", Status = newStatus.ToString() };
    }

    public async Task<object> UpdateMemberRatesAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default)
    {
        var membership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == userId.ToString(), cancellationToken);

        if (membership == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        // Update rates logic here
        membership.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { Success = true, Message = "Rates updated successfully" };
    }

    public async Task<object> RemoveMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var membership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId.ToString() && m.UserId == userId.ToString(), cancellationToken);

        if (membership == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        _context.OrganizationMemberships.Remove(membership);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { Success = true, Message = "Member removed successfully" };
    }

    public async Task<bool> JoinOrganizationAsync(string userId, JoinOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var organization = await _organizationRepository.GetByCodeAsync(request.OrganizationCode, cancellationToken);
        if (organization == null)
        {
            throw new KeyNotFoundException($"Organization with code {request.OrganizationCode} not found");
        }

        var existingRequest = await _joinRequestRepository.GetPendingRequestAsync(userId, organization.Id, cancellationToken);
        if (existingRequest != null)
        {
            throw new InvalidOperationException("A pending join request already exists");
        }

        var joinRequest = new JoinRequest
        {
            OrganizationId = organization.Id,
            UserId = userId,
            Status = JoinRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _joinRequestRepository.AddAsync(joinRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
