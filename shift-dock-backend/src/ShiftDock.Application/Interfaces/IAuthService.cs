using ShiftDock.Application.DTOs.Auth;

namespace ShiftDock.Application.Interfaces;

public interface IAuthService
{
    Task<bool> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
    Task<bool> ResendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> SignOutAsync(Guid userId, CancellationToken cancellationToken = default);
}
