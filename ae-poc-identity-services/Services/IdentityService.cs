using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Ae.Poc.Identity.Interfaces;
using Ae.Poc.Identity.Authentication;
using Ae.Poc.Identity.Data;
using System.Security.Claims;

namespace Ae.Poc.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly IIdentityStorageService _storage;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public IdentityService(
       ILogger<IdentityService> logger,
       IIdentityStorageService storage,
       IMapper mapper,
       ITokenService tokenService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<ClientCredentialsResult> TryVerifyClientCredentialAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        _logger.LogDebug("Start {ServiceName} {MethodName}() ...", nameof(IdentityService), nameof(TryVerifyClientCredentialAsync));

        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Incorrect arguments username or password. '{Email}'. {ServiceName} {MethodName}()",
                    email, nameof(IdentityService), nameof(TryVerifyClientCredentialAsync));
                return new ClientCredentialsResult(isVerified: false, infoMessage: "Incorrect arguments username or password.");
            }

            // Verify the credential
            (bool success, AccountIdentity? accountIdentity) = await _storage.TryGetAccountIdentityByEmailAsync(email, ct);
            if (!success)
            {
                _logger.LogWarning("User not found '{Email}'.", email);
                return new ClientCredentialsResult(isVerified: false, infoMessage: $"User not found '{email}'.");
            }

            // Check if the account is locked
            if (accountIdentity!.IsLocked)
            {
                _logger.LogWarning("Account is locked '{Email}'.", email);
                return new ClientCredentialsResult(isVerified: false, infoMessage: $"Account is locked '{email}'.");
            }

            if (new PasswordHasher<AccountIdentity>().VerifyHashedPassword(accountIdentity!, accountIdentity!.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("PASSWORD NOT VERIFIED '{Email}'.", email);
                return new ClientCredentialsResult(isVerified: false, infoMessage: $"The password is incorrect '{email}'.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, accountIdentity.Id.ToString()),
                new(ClaimTypes.Email, accountIdentity.EmailAddress),
                new(ClaimTypes.Name, accountIdentity.EmailAddress) // Or a dedicated Name property if available
            };

            string? accessToken = _tokenService.GenerateAccessToken(claims);
            string? refreshToken = _tokenService.GenerateRefreshToken();
            string? tokenType = "Bearer";
            int? expiresIn = 60; // Assuming minutes from TokenService options, simplified for now
            int? refreshTokenExpiresIn = 1440; // 1 day

            return new ClientCredentialsResult(
                isVerified: true,
                infoMessage: "Ok.",
                accessToken: accessToken,
                refreshToken: refreshToken,
                tokenType: tokenType,
                expiresIn: expiresIn,
                refreshTokenExpiresIn: refreshTokenExpiresIn);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{ServiceName} {MethodName}() ", nameof(IdentityService), nameof(TryVerifyClientCredentialAsync));
            return new ClientCredentialsResult(isVerified: false, infoMessage: $"Error '{exc.Message}'.");
        }
    }
}
