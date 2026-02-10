using AutoMapper;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Users;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;

namespace ShiftDock.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetUserByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByPhoneAsync(phone, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with phone {phone} not found");
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this phone number already exists");
        }

        var user = new User
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
        }

        if (request.DateOfBirth.HasValue)
        {
            user.DateOfBirth = request.DateOfBirth;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetOrganizationMembersAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByOrganizationIdAsync(organizationId.ToString(), 1, 1000, cancellationToken);
        return _mapper.Map<List<UserResponse>>(users);
    }

    public async Task<UserResponse> AddWorkerAsync(Guid organizationId, Guid currentUserId, CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement worker addition logic with organization membership
        return await CreateUserAsync(request, cancellationToken);
    }

    public async Task<UserResponse> UpdateMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Add authorization check
        return await UpdateUserAsync(userId, request, cancellationToken);
    }

    public async Task<UserResponse> UpdateMemberStatusAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement status update logic
        var user = await _userRepository.GetByIdAsync(userId.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> RemoveMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement member removal logic
        await Task.CompletedTask;
        return true;
    }

    public async Task<object> GetNotificationPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement notification preferences retrieval
        await Task.CompletedTask;
        return new { };
    }

    public async Task<object> UpdateNotificationPreferencesAsync(Guid userId, object request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement notification preferences update
        await Task.CompletedTask;
        return new { };
    }

    public async Task<PaginatedResponse<UserResponse>> GetOrganizationMembersAsync(string organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByOrganizationIdAsync(organizationId, page, pageSize, cancellationToken);
        var totalCount = await _userRepository.GetCountByOrganizationIdAsync(organizationId, cancellationToken);

        var userResponses = _mapper.Map<List<UserResponse>>(users);

        return new PaginatedResponse<UserResponse>
        {
            Items = userResponses,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
