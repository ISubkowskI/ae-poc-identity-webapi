using Ae.Poc.Identity.Services;
using Ae.Poc.Identity.Interfaces;
using Ae.Poc.Identity.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using System.Security.Claims;
using FluentAssertions;

namespace Ae.Poc.Identity.Unittests.Services;

public class IdentityServiceTests
{
    private readonly Mock<ILogger<IdentityService>> _mockLogger;
    private readonly Mock<IIdentityStorageService> _mockStorage;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly IdentityService _service;

    public IdentityServiceTests()
    {
        _mockLogger = new Mock<ILogger<IdentityService>>();
        _mockStorage = new Mock<IIdentityStorageService>();
        _mockMapper = new Mock<IMapper>();
        _mockTokenService = new Mock<ITokenService>();

        _service = new IdentityService(
            _mockLogger.Object,
            _mockStorage.Object,
            _mockMapper.Object,
            _mockTokenService.Object
        );
    }

    [Fact]
    public async Task TryVerifyClientCredentialAsync_InvalidArgs_ReturnsFalse()
    {
        // Act
        var result = await _service.TryVerifyClientCredentialAsync("", "password");

        // Assert
        result.IsVerified.Should().BeFalse();
        result.InfoMessage.Should().Contain("Incorrect arguments");
    }

    [Fact]
    public async Task TryVerifyClientCredentialAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        _mockStorage
            .Setup(s => s.TryGetAccountIdentityByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, null));

        // Act
        var result = await _service.TryVerifyClientCredentialAsync("email@test.com", "password");

        // Assert
        result.IsVerified.Should().BeFalse();
        result.InfoMessage.Should().Contain("User not found");
    }

    [Fact]
    public async Task TryVerifyClientCredentialAsync_AccountLocked_ReturnsFalse()
    {
        // Arrange
        var account = new AccountIdentity { IsLocked = true, EmailAddress = "test@test.com" };
        _mockStorage
            .Setup(s => s.TryGetAccountIdentityByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, account));

        // Act
        var result = await _service.TryVerifyClientCredentialAsync("email@test.com", "password");

        // Assert
        result.IsVerified.Should().BeFalse();
        result.InfoMessage.Should().Contain("Account is locked");
    }

    [Fact]
    public async Task TryVerifyClientCredentialAsync_WrongPassword_ReturnsFalse()
    {
        // Arrange
        var hasher = new PasswordHasher<AccountIdentity>();
        var account = new AccountIdentity { EmailAddress = "test@test.com" };
        account.PasswordHash = hasher.HashPassword(account, "correct_password");
        
        _mockStorage
            .Setup(s => s.TryGetAccountIdentityByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, account));

        // Act
        var result = await _service.TryVerifyClientCredentialAsync("email@test.com", "wrong_password");

        // Assert
        result.IsVerified.Should().BeFalse();
        result.InfoMessage.Should().Contain("password is incorrect");
    }

    [Fact]
    public async Task TryVerifyClientCredentialAsync_Success_ReturnsTokens()
    {
        // Arrange
        var hasher = new PasswordHasher<AccountIdentity>();
        var account = new AccountIdentity { Id = Guid.NewGuid(), EmailAddress = "test@test.com" };
        account.PasswordHash = hasher.HashPassword(account, "password");

        _mockStorage
            .Setup(s => s.TryGetAccountIdentityByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, account));

        _mockTokenService.Setup(t => t.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns("access_token");
        _mockTokenService.Setup(t => t.GenerateRefreshToken()).Returns("refresh_token");

        // Act
        var result = await _service.TryVerifyClientCredentialAsync("email@test.com", "password");

        // Assert
        result.IsVerified.Should().BeTrue();
        result.InfoMessage.Should().Be("Ok.");
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
    }
}
