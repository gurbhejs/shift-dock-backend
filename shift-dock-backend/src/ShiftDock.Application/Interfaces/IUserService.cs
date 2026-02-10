using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.DTOs.Users;

namespace ShiftDock.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserResponse> GetUserByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetOrganizationMembersAsync(Guid organizationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<UserResponse> AddWorkerAsync(Guid organizationId, Guid currentUserId, CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateMemberStatusAsync(Guid organizationId, Guid userId, Guid currentUserId, object request, CancellationToken cancellationToken = default);
    Task<bool> RemoveMemberAsync(Guid organizationId, Guid userId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<object> GetNotificationPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<object> UpdateNotificationPreferencesAsync(Guid userId, object request, CancellationToken cancellationToken = default);
}
