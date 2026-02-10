using ShiftDock.Application.DTOs.Auth;
using ShiftDock.Application.DTOs.Users;
using ShiftDock.Application.Interfaces;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Exceptions;

namespace ShiftDock.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly CognitoService _cognitoService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        CognitoService cognitoService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _cognitoService = cognitoService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user exists
        var user = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (user == null)
        {
            throw new UserNotFoundException("User not found. Please sign up first.");
        }

        return await _cognitoService.SendOtpAsync(request.Phone, cancellationToken);
    }

    public async Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var (accessToken, refreshToken, idToken) = await _cognitoService.VerifyOtpAsync(
            request.Phone, 
            request.Otp, 
            cancellationToken
        );

        var user = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        bool isNewUser = false;

        if (user == null)
        {
            // Create new user if not exists
            user = new User
            {
                Phone = request.Phone,
                Name = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            isNewUser = true;
        }

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            IsNewUser = isNewUser,
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
        var existingUser = await _userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists");
        }

        var attributes = new Dictionary<string, string>
        {
            { "name", request.Name }
        };

        if (!string.IsNullOrEmpty(request.Email))
        {
            attributes.Add("email", request.Email);
        }

        var (accessToken, refreshToken, idToken) = await _cognitoService.SignUpAsync(
            request.Phone,
            GenerateRandomPassword(),
            attributes,
            cancellationToken
        );

        var user = new User
        {
            Phone = request.Phone,
            Name = request.Name,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

    public async Task<bool> ResendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        return await _cognitoService.SendOtpAsync(request.Phone, cancellationToken);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var (accessToken, idToken) = await _cognitoService.RefreshTokenAsync(
            request.RefreshToken, 
            cancellationToken
        );

        return new TokenResponse
        {
            Token = accessToken,
            RefreshToken = request.RefreshToken
        };
    }

    public async Task<bool> SignOutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement sign out logic (invalidate refresh tokens, etc.)
        await Task.CompletedTask;
        return true;
    }

    private string GenerateRandomPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
