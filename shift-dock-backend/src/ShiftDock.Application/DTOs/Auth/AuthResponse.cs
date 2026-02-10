using ShiftDock.Application.DTOs.Users;

namespace ShiftDock.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserResponse User { get; set; } = null!;
    public bool IsNewUser { get; set; }
}
