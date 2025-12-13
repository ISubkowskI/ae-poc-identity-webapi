namespace Ae.Poc.Identity.Settings;

public sealed class IdentityApiOptions
{
    public const string App = "App";

    public string Title { get; set; } = String.Empty;
    public string Version { get; set; } = String.Empty;
    //public string BaseUrl { get; set; } = String.Empty;
    public string ClientId { get; set; } = String.Empty;
}

