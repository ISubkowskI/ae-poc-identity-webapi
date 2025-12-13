using Ae.Poc.Identity.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Ae.Poc.Identity.IntegrationTests;

public class IdentityAccountsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IdentityAccountsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAccounts_ReturnsOk_AndList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/accounts");

        // Assert
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<IEnumerable<AccountIdentityOutgoingDto>>();
        Assert.NotNull(accounts);
        // Note: Actual count depends on Seed() method, but we expect at least empty or seeded data.
    }

    [Fact]
    public async Task Register_ValidAccount_ReturnsOk_AndResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Since we use in-memory DB and it's transient per test run usually (or shared depending on setup), 
        // we use a random email to avoid conflicts if valid for parallel.
        var dto = new AccountRegistrationIncomingDto
        {
             Email = $"test_{Guid.NewGuid()}@example.com",
             Password = "Password123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/accounts", dto);

        // Assert
        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}. Content: {content}");
        var result = await response.Content.ReadFromJsonAsync<Ae.Poc.Identity.Data.OperationResult<Ae.Poc.Identity.Data.AccountRegistrationResult>>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }
    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dto = new AccountRegistrationIncomingDto { Email = "invalid_email", Password = "Pass" };

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/accounts", dto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ExistingUser_ReturnsFailure()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"duplicate_{Guid.NewGuid()}@example.com";
        var dto = new AccountRegistrationIncomingDto { Email = email, Password = "Password123!" };

        // 1. Register first time
        var res1 = await client.PostAsJsonAsync("/api/v2/accounts", dto);
        res1.EnsureSuccessStatusCode();

        // Act
        // 2. Register second time
        var res2 = await client.PostAsJsonAsync("/api/v2/accounts", dto);

        // Assert
        // Controller returns BadRequest(result.InfoMessage) on failure.
        res2.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var content = await res2.Content.ReadAsStringAsync();
        content.Should().Contain("exists");
    }
}
