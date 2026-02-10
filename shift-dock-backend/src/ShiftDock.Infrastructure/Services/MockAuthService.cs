using ShiftDock.Application.DTOs.Auth;
using ShiftDock.Application.DTOs.Users;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShiftDock.Infrastructure.Configuration;
using ShiftDock.Domain.Exceptions;

namespace ShiftDock.Infrastructure.Services;

/// <summary>
/// Mock authentication service for local development (bypasses AWS Cognito)
/// </summary>
public class MockAuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly Dictionary<string, string> _otpStore = new(); // Store OTPs in memory

    public MockAuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<bool> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (user == null)
        {
            throw new UserNotFoundException("User not found. Please sign up first.");
        }
        // Mock: Store OTP as "123456" for all phone numbers
        var otp = "123456";
        _otpStore[request.Phone] = otp;
        Console.WriteLine($"[MOCK] OTP for {request.Phone}: {otp}");
        return true;
    }

    public Task<bool> ResendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        // Mock: Same as SendOtpAsync
        return SendOtpAsync(request, cancellationToken);
    }

    public async Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        // Mock: Accept any OTP or "123456"
        if (!_otpStore.ContainsKey(request.Phone) && request.Otp != "123456")
        {
            throw new UnauthorizedAccessException("Invalid OTP");
        }

        // Check if user exists
        var user = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found. Please sign up first.");
        }

        // Generate tokens
        var (accessToken, refreshToken, idToken) = GenerateTokens(user);

        // Clean up OTP
        _otpStore.Remove(request.Phone);

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            IsNewUser = false,
            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Phone = request.Phone,
            Name = request.Name,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var (accessToken, refreshToken, idToken) = GenerateTokens(user);

        // Clean up OTP
        _otpStore.Remove(request.Phone);

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            IsNewUser = true,
            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Mock: Accept any refresh token and return new tokens
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(request.RefreshToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Generate new tokens
            var (accessToken, refreshToken, idToken) = GenerateTokens(userId);

            return Task.FromResult(new TokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
        catch
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }
    }

    public Task<bool> SignOutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Mock: Just log the sign out
        Console.WriteLine($"[MOCK] User {userId} signed out");
        return Task.FromResult(true);
    }

    private (string accessToken, string refreshToken, string idToken) GenerateTokens(User user)
    {
        return GenerateTokens(user.Id);
    }

    private (string accessToken, string refreshToken, string idToken) GenerateTokens(string userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var accessToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        var refreshToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            signingCredentials: credentials
        );

        var idToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return (
            tokenHandler.WriteToken(accessToken),
            tokenHandler.WriteToken(refreshToken),
            tokenHandler.WriteToken(idToken)
        );
    }
}
