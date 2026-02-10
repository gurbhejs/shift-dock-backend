using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftDock.Application.DTOs.Auth;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Application.Interfaces;

namespace ShiftDock.API.Controllers;

/// <summary>
/// Handles authentication operations including OTP verification and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Sends OTP to the provided phone number
    /// </summary>
    /// <param name="request">Phone number to send OTP to</param>
    /// <returns>Success message</returns>
    [HttpPost("send-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> SendOtp([FromBody] SendOtpRequest request)
    {
        var result = await _authService.SendOtpAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Verifies the OTP sent to the phone number
    /// </summary>
    /// <param name="request">Phone number and OTP code</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Completes user registration after OTP verification
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> SignUp([FromBody] SignUpRequest request)
    {
        var result = await _authService.SignUpAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Resends OTP to the phone number
    /// </summary>
    /// <param name="request">Phone number to resend OTP to</param>
    /// <returns>Success message</returns>
    [HttpPost("resend-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ResendOtp([FromBody] SendOtpRequest request)
    {
        var result = await _authService.ResendOtpAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Refreshes the access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New token pair</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Signs out the current user and invalidates their refresh token
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("signout")]
    [Authorize]
    public new async Task<ActionResult<ApiResponse<object>>> SignOut()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _authService.SignOutAsync(Guid.Parse(userId));
        return Ok(result);
    }
}
