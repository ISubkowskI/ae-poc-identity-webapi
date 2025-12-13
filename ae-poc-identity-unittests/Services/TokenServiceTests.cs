using Ae.Poc.Identity.Services;
using Ae.Poc.Identity.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;
using FluentAssertions;

namespace Ae.Poc.Identity.Unittests.Services;

public class TokenServiceTests
{
    private readonly Mock<ILogger<TokenService>> _mockLogger;
    private readonly Mock<IOptions<IdentityTokenOptions>> _mockOptions;
    private readonly IdentityTokenOptions _tokenOptions;
    private readonly TokenService _service;

    public TokenServiceTests()
    {
        _mockLogger = new Mock<ILogger<TokenService>>();
        _tokenOptions = new IdentityTokenOptions
        {
            SecretKey = "SuperSecretKeyForTestingPurposesOnly12345!_Must_Be_Very_Long_Indeed_To_Satisfy_HMACSHA512_Requirement_Of_64_Bytes", // Must be long enough for HmacSha512
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessExpiresInMinutes = 60
        };
        _mockOptions = new Mock<IOptions<IdentityTokenOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(_tokenOptions);

        _service = new TokenService(_mockLogger.Object, _mockOptions.Object);
    }

    [Fact]
    public void GenerateAccessToken_WithClaims_ReturnsToken()
    {
        // Arrange
        var claims = new List<Claim> { new("sub", "123") };

        // Act
        var token = _service.GenerateAccessToken(claims);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Should().HaveCount(3); // Header.Payload.Signature
    }

    [Fact]
    public void GenerateAccessToken_WithIdentity_ReturnsToken()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[] { new Claim("sub", "123") });

        // Act
        var token = _service.GenerateAccessToken(identity);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsRandomString()
    {
        // Act
        var token1 = _service.GenerateRefreshToken();
        var token2 = _service.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrWhiteSpace();
        token2.Should().NotBeNullOrWhiteSpace();
        token1.Should().NotBe(token2);
    }
}
