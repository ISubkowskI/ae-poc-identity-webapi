using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.Dtos;
using Ae.Poc.Identity.Authentication;
using Ae.Poc.Identity.DbEntities;
using Ae.Poc.Identity.Interfaces;
using Ae.Poc.Identity.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace Ae.Poc.Identity.IntegrationTests;

public class IdentityLoginIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IdentityLoginIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = $"login_valid_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        
        // Seed user via service resolve
        using (var scope = _factory.Services.CreateScope())
        {
            var storage = scope.ServiceProvider.GetRequiredService<IIdentityStorageService>();
            var hasher = new PasswordHasher<AccountIdentity>();
            var account = new AccountIdentity { Id = Guid.NewGuid(), EmailAddress = email };
            account.PasswordHash = hasher.HashPassword(account, password);
            await storage.CreateAccountAsync(account);
        }

        var loginDto = new LoginRequestDto { Email = email, Password = password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v2/identity/token", loginDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var result = await response.Content.ReadFromJsonAsync<ClientCredentialsResult>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized() 
    {
        // Arrange
        var loginDto = new LoginRequestDto { Email = "nonexistent@example.com", Password = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v2/identity/token", loginDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var result = await response.Content.ReadFromJsonAsync<ClientCredentialsResult>();
        result.Should().NotBeNull();
        result!.IsVerified.Should().BeFalse();
    }
}
