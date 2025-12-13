using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ae.Poc.Identity.IntegrationTests;

public class IdentityControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IdentityControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetDiscoveryDocument_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/identity/.well-known/openid-configuration");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected success, but got {response.StatusCode}. Content: {content}");
        Assert.Equal("\"ToDo\"", content);
    }
}
