namespace Ae.Poc.Identity.Authentication;

/// <summary>
/// Represents the result of a client credentials verification operation.
/// </summary>
/// <remarks>
/// This class encapsulates the outcome of verifying client credentials, including 
/// whether the verification was successful, relevant tokens, and their expiration details.
/// </remarks>
/// <param name="isVerified">Indicates whether the credentials were successfully verified.</param>
/// <param name="infoMessage">An informational message about the verification result.</param>
/// <param name="accessToken">The access token issued upon successful verification.</param>
/// <param name="refreshToken">The refresh token issued upon successful verification.</param>
/// <param name="tokenType">The type of the issued token (e.g., Bearer).</param>
/// <param name="expiresIn">The duration in seconds until the access token expires.</param>
/// <param name="refreshTokenExpiresIn">The duration in seconds until the refresh token expires.</param>
public sealed class ClientCredentialsResult(
    bool isVerified,
    string infoMessage,
    string? accessToken = null,
    string? refreshToken = null,
    string? tokenType = null,
    int? expiresIn = null,
    int? refreshTokenExpiresIn = null)
{
    /// <summary>
    /// Gets a value indicating whether the credentials were successfully verified.
    /// </summary>
    public bool IsVerified { get; init; } = isVerified;

    /// <summary>
    /// Gets an informational message about the verification result.
    /// </summary>
    /// <remarks>
    /// This message may provide additional context, such as error details if verification failed.
    /// </remarks>
    public string InfoMessage { get; init; } = infoMessage;

    /// <summary>
    /// Gets the access token issued upon successful verification.
    /// </summary>
    public string? AccessToken { get; init; } = accessToken;

    /// <summary>
    /// Gets the refresh token issued upon successful verification.
    /// </summary>
    public string? RefreshToken { get; init; } = refreshToken;

    /// <summary>
    /// Gets the type of the issued token (e.g., Bearer).
    /// </summary>
    public string? TokenType { get; init; } = tokenType;

    /// <summary>
    /// Gets the duration in seconds until the access token expires.
    /// </summary>
    public int? ExpiresIn { get; init; } = expiresIn;

    /// <summary>
    /// Gets the duration in seconds until the refresh token expires.
    /// </summary>
    public int? RefreshTokenExpiresIn { get; init; } = refreshTokenExpiresIn;
}
