using Ae.Poc.Identity.Security;
using System.Security.Claims;
using System.Text.Json;

namespace Ae.Poc.Identity.Utils;

public static class ClaimUtils
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = false };

    public static string SerializeToJson(IEnumerable<Claim> claims)
    {
        return JsonSerializer.Serialize(claims.Select(c => new ClaimSaver
        {
            Type = c.Type,
            Value = c.Value,
            ValueType = c.ValueType,
            Issuer = c.Issuer,
            OriginalIssuer = c.OriginalIssuer,
            Properties = c.Properties.ToDictionary(p => p.Key, p => p.Value)
        }),
        JsonSerializerOptions);
    }

    public static IEnumerable<Claim> DeserializeFromJson(string json)
    {
        var records = JsonSerializer.Deserialize<IEnumerable<ClaimSaver>>(json, JsonSerializerOptions);
        if (records == null)
        {
            return [];
        }

        var claims = records.Select(r =>
        {
            var claim = new Claim(r.Type, r.Value, r.ValueType, r.Issuer, r.OriginalIssuer);
            if (r.Properties != null)
            {
                foreach (var kvp in r.Properties)
                    claim.Properties[kvp.Key] = kvp.Value;
            }
            return claim;
        }).ToList();
        return claims;
        // ?? Enumerable.Empty<Claim>();
    }

    public static string SerializePropertiesToJson(IDictionary<string, string>? properties)
    {
        if (properties == null || !properties.Any())
            return "{}";

        return JsonSerializer.Serialize(properties);
    }

    internal static IDictionary<string, string>? DeserializePropertiesFromJson(string? propertiesJson)
    {
        if (string.IsNullOrWhiteSpace(propertiesJson) || propertiesJson == "{}")
            return null;

        return JsonSerializer.Deserialize<IDictionary<string, string>>(propertiesJson, JsonSerializerOptions);
    }
}
