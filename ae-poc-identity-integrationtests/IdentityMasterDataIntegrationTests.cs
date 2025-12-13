using Ae.Poc.Identity.Dtos;
using Ae.Poc.Identity.DbEntities; // If needing raw access, but mostly DTOs
using Ae.Poc.Identity.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Ae.Poc.Identity.IntegrationTests;

public class IdentityMasterDataIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IdentityMasterDataIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateClaim_Valid_ReturnsCreated()
    {
        // Arrange
        var claim = new AppClaimIncomingDto
        {
            Type = $"TestType_{Guid.NewGuid()}",
            Value = "TestValue",
            Description = "Test Description",
            ValueType = "string"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v2/masterdata/claims", claim);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        created.Should().NotBeNull();
        created!.Type.Should().Be(claim.Type);
        created.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetClaims_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v2/masterdata/claims?skipped=0&numberOf=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var claims = await response.Content.ReadFromJsonAsync<IEnumerable<AppClaimOutgoingDto>>();
        claims.Should().NotBeNull();
    }

    [Fact]
    public async Task GetClaim_Existing_ReturnsOk()
    {
        // Seeding first
        var claim = new AppClaimIncomingDto
        {
            Type = $"GetTest_{Guid.NewGuid()}",
            Value = "Val",
            ValueType = "string"
        };
        var resPost = await _client.PostAsJsonAsync("/api/v2/masterdata/claims", claim);
        resPost.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await resPost.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        var id = created!.Id;

        // Act
        var response = await _client.GetAsync($"/api/v2/masterdata/claims/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await response.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(id);
    }

    [Fact]
    public async Task UpdateClaim_Valid_ReturnsOk()
    {
        // Seeding
        var claim = new AppClaimIncomingDto
        {
            Type = $"UpdTest_{Guid.NewGuid()}",
            Value = "Val",
            ValueType = "string"
        };
        var resPost = await _client.PostAsJsonAsync("/api/v2/masterdata/claims", claim);
        var created = await resPost.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        var id = created!.Id;

        var updateDto = new AppClaimIncomingDto
        {
            Id = id,
            Type = created.Type, // Type cannot change usually, kept same
            Value = "UpdatedVal",
            ValueType = "string",
            Description = "Updated Desc"
        };

        // Act
        var response = await _client.PatchAsync($"/api/v2/masterdata/claims/{id}", JsonContent.Create(updateDto)); // Controller uses HttpPatch

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        updated!.Value.Should().Be("UpdatedVal");
        updated.Description.Should().Be("Updated Desc");
    }

    [Fact]
    public async Task DeleteClaim_Existing_ReturnsOk()
    {
         // Seeding
        var claim = new AppClaimIncomingDto
        {
            Type = $"DelTest_{Guid.NewGuid()}",
            Value = "Val",
            ValueType = "string"
        };
        var resPost = await _client.PostAsJsonAsync("/api/v2/masterdata/claims", claim);
        var created = await resPost.Content.ReadFromJsonAsync<AppClaimOutgoingDto>();
        var id = created!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/v2/masterdata/claims/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify gone
        var resGet = await _client.GetAsync($"/api/v2/masterdata/claims/{id}");
        resGet.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
