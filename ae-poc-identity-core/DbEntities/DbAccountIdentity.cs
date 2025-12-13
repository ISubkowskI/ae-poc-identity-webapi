using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ae.Poc.Identity.DbEntities;

[Table("AccountIdentities")]
public partial class DbAccountIdentity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, EmailAddress]
    public string EmailAddress { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsLocked { get; set; } = false;
    [Required]
    public string ClaimsJson { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateOnly EmploymentDate { get; set; } = DateOnly.MinValue;
    public DateOnly EmploymentExpiredDate { get; set; } = DateOnly.MinValue;

    public DateTimeOffset LastLoginUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset LastPasswordChangeUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset PasswordExpiredOnUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset EmailVerifiedOnUtc { get; set; } = DateTimeOffset.MinValue;

    public ICollection<DbRefreshToken> RefreshTokens { get; set; } = [];


    // Factory method for creating a new DbAccountIdentity
    public static DbAccountIdentity Create(
        string emailAddress,
        string passwordHash,
        string displayName,
        string claimsJson,
        DateOnly employmentDate,
        DateOnly employmentExpiredDate)
    {
        return new DbAccountIdentity
        {
            EmailAddress = emailAddress,
            PasswordHash = passwordHash,
            DisplayName = displayName,
            ClaimsJson = claimsJson,
            EmploymentDate = employmentDate,
            EmploymentExpiredDate = employmentExpiredDate,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastLoginUtc = DateTimeOffset.MinValue,
            LastPasswordChangeUtc = DateTimeOffset.UtcNow,
            PasswordExpiredOnUtc = DateTimeOffset.UtcNow.AddMonths(3),
            EmailVerifiedOnUtc = DateTimeOffset.MinValue
        };
    }
}
