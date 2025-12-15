namespace Ae.Poc.Identity.Settings;

public sealed class IdentityApiOptions
{
    public const string App = "App";

    public string Title { get; set; } = String.Empty;
    /// <summary>
    /// The version of the application. (e.g. "1.0.1")
    /// </summary>
    public string Version { get; set; } = "?.?";
    //public string BaseUrl { get; set; } = String.Empty;
    public string ClientId { get; set; } = "???";
}

