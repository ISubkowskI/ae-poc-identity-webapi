using System.Globalization;
using System.Security.Claims;

namespace Ae.Poc.Identity.Data;

public sealed class AccountIdentity
{
    /// <summary>
    /// Gets or sets the unique identifier for the account.
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the email address associated with the account.
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed password for the account.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name for the account.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description for the account.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the account is locked.
    /// </summary>
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// Gets or sets the claims associated with the account.
    /// </summary>
    public IEnumerable<Claim> Claims { get; set; } = [];

    /// <summary>
    /// Gets or sets the timestamp when the account was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the employment date of the account holder.
    /// </summary>
    public DateOnly EmploymentDate { get; set; } = DateOnly.MinValue;

    /// <summary>
    /// Gets or sets the date when the employment is expected to expire.
    /// </summary>
    public DateOnly EmploymentExpiredDate { get; set; } = DateOnly.MinValue;

    /// <summary>
    /// Gets or sets the timestamp of the last login.
    /// </summary>
    public DateTimeOffset LastLoginUtc { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Gets or sets the timestamp of the last password change.
    /// </summary>
    public DateTimeOffset LastPasswordChangeUtc { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Gets or sets the timestamp when the current password will expire.
    /// </summary>
    public DateTimeOffset PasswordExpiredOnUtc { get; set; } = DateTimeOffset.MaxValue;

    /// <summary>
    /// Gets or sets the timestamp when the email was verified.
    /// </summary>
    public DateTimeOffset EmailVerifiedOnUtc { get; set; } = DateTimeOffset.MinValue;


    /// <summary>
    /// Formats the employment date as a string in "yyyy-MM-dd" format.
    /// </summary>
    /// <returns>The formatted employment date string.</returns>
    public string ToStringEmploymentDate() => EmploymentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
