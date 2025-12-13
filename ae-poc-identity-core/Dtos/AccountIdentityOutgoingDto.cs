namespace Ae.Poc.Identity.Dtos;

public sealed record AccountIdentityOutgoingDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsLocked { get; set; } = false;
    public string ClaimsJson { get; set; } = "[]";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateOnly EmploymentDate { get; set; } = DateOnly.MinValue;
    public DateOnly EmploymentExpiredDate { get; set; } = DateOnly.MinValue;
    public DateTimeOffset LastLoginUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset LastPasswordChangeUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset PasswordExpiredOnUtc { get; set; } = DateTimeOffset.MaxValue;
    public DateTimeOffset EmailVerifiedOnUtc { get; set; } = DateTimeOffset.MinValue;
}
