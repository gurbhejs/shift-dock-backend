using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using ShiftDock.Infrastructure.Configuration;

namespace ShiftDock.Infrastructure.Services;

public class CognitoService
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly AwsSettings _awsSettings;

    public CognitoService(IOptions<AwsSettings> awsSettings)
    {
        _awsSettings = awsSettings.Value;
        _cognitoClient = new AmazonCognitoIdentityProviderClient(
            _awsSettings.AccessKeyId,
            _awsSettings.SecretAccessKey,
            Amazon.RegionEndpoint.GetBySystemName(_awsSettings.Region)
        );
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new AdminCreateUserRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                Username = phoneNumber,
                TemporaryPassword = GenerateTemporaryPassword(),
                MessageAction = MessageActionType.SUPPRESS,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "phone_number", Value = phoneNumber },
                    new AttributeType { Name = "phone_number_verified", Value = "true" }
                }
            };

            await _cognitoClient.AdminCreateUserAsync(request, cancellationToken);
            return true;
        }
        catch (UsernameExistsException)
        {
            // User already exists, send custom OTP via SMS
            // Implementation would depend on your SMS service
            return true;
        }
        catch (Exception ex)
        {
            // Log exception
            throw new InvalidOperationException("Failed to send OTP", ex);
        }
    }

    public async Task<(string AccessToken, string RefreshToken, string IdToken)> VerifyOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                ClientId = _awsSettings.ClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", phoneNumber },
                    { "PASSWORD", otp }
                }
            };

            var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest, cancellationToken);

            return (
                authResponse.AuthenticationResult.AccessToken,
                authResponse.AuthenticationResult.RefreshToken,
                authResponse.AuthenticationResult.IdToken
            );
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid OTP or phone number", ex);
        }
    }

    public async Task<(string AccessToken, string RefreshToken, string IdToken)> SignUpAsync(string phoneNumber, string password, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
    {
        try
        {
            var userAttributes = new List<AttributeType>
            {
                new AttributeType { Name = "phone_number", Value = phoneNumber },
                new AttributeType { Name = "phone_number_verified", Value = "true" }
            };

            foreach (var attr in attributes)
            {
                userAttributes.Add(new AttributeType { Name = attr.Key, Value = attr.Value });
            }

            var createUserRequest = new AdminCreateUserRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                Username = phoneNumber,
                TemporaryPassword = password,
                MessageAction = MessageActionType.SUPPRESS,
                UserAttributes = userAttributes
            };

            await _cognitoClient.AdminCreateUserAsync(createUserRequest, cancellationToken);

            var setPasswordRequest = new AdminSetUserPasswordRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                Username = phoneNumber,
                Password = password,
                Permanent = true
            };

            await _cognitoClient.AdminSetUserPasswordAsync(setPasswordRequest, cancellationToken);

            var authRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                ClientId = _awsSettings.ClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", phoneNumber },
                    { "PASSWORD", password }
                }
            };

            var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest, cancellationToken);

            return (
                authResponse.AuthenticationResult.AccessToken,
                authResponse.AuthenticationResult.RefreshToken,
                authResponse.AuthenticationResult.IdToken
            );
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to sign up user", ex);
        }
    }

    public async Task<(string AccessToken, string IdToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _awsSettings.UserPoolId,
                ClientId = _awsSettings.ClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            var response = await _cognitoClient.AdminInitiateAuthAsync(request, cancellationToken);

            return (
                response.AuthenticationResult.AccessToken,
                response.AuthenticationResult.IdToken
            );
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid refresh token", ex);
        }
    }

    private string GenerateTemporaryPassword()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
