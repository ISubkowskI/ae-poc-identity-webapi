namespace Ae.Poc.Identity.Data;

public sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid AccountIdentityId { get; set; } = Guid.Empty;
    public string TokenHash { get; set; } = null!;

    public DateTimeOffset ExpiresUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset? RevokedUtc { get; set; } = DateTimeOffset.MinValue;

    public string CreatedByIp { get; set; } = null!;
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresUtc;
    public bool IsActive => RevokedUtc == null && !IsExpired;
}
