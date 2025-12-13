using Ae.Poc.Identity.Utils;
using System.Security.Claims;
using Xunit;

namespace Ae.Poc.Identity.Unittests.Utils;

public class ClaimUtilsTests
{
    [Fact]
    public void SerializeToJson_ShouldSerializeClaimsCorrectly()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("type1", "value1"),
            new Claim("type2", "value2")
        };

        // Act
        var json = ClaimUtils.SerializeToJson(claims);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("type1", json);
        Assert.Contains("value1", json);
    }

    [Fact]
    public void DeserializeFromJson_ShouldDeserializeClaimsCorrectly()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("type1", "value1"),
            new Claim("type2", "value2")
        };
        var json = ClaimUtils.SerializeToJson(claims);

        // Act
        var deserializedClaims = ClaimUtils.DeserializeFromJson(json).ToList();

        // Assert
        Assert.NotNull(deserializedClaims);
        Assert.Equal(2, deserializedClaims.Count);
        Assert.Contains(deserializedClaims, c => c.Type == "type1" && c.Value == "value1");
        Assert.Contains(deserializedClaims, c => c.Type == "type2" && c.Value == "value2");
    }
}
