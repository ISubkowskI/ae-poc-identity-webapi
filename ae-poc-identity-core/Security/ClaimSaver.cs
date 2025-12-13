using System.Security.Claims;

namespace Ae.Poc.Identity.Security;

internal class ClaimSaver
{
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string ValueType { get; set; } = ClaimValueTypes.String;
    public string Issuer { get; set; } = "LOCAL AUTHORITY";
    public string OriginalIssuer { get; set; } = "LOCAL AUTHORITY";
    public Dictionary<string, string>? Properties { get; set; }
}
