using System.ComponentModel.DataAnnotations.Schema;

namespace Ae.Poc.Identity.DbEntities;

[Table("RefreshTokens")]
public partial class DbRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountIdentityId { get; set; } // Foreign key to DbAccountIdentity
    public string TokenHash { get; set; } = null!;

    public DateTimeOffset ExpiresUtc { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? RevokedUtc { get; set; }

    public string CreatedByIp { get; set; } = null!;
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }
}
