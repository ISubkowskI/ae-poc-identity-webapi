namespace Ae.Poc.Identity.Settings;

public sealed class IdentityTokenOptions
{
    public const string IdentityToken = "IdentityToken";

    public string SecretKey { get; set; } = String.Empty;
    public string Issuer { get; set; } = String.Empty;
    public string Audience { get; set; } = String.Empty;
    public int AccessExpiresInMinutes { get; set; } = 15;
    public int RefreshExpiresIn { get; set; } = 10;  // in days
}

