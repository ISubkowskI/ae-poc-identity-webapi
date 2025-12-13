using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ae.Poc.Identity.DbEntities;

[Table("AppClaims")]
public partial class DbAppClaim
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;
    public string ValueType { get; set; } = null!;
    public string DisplayText { get; set; } = null!;
    public string? PropertiesJson { get; set; }
    public string? Description { get; set; }
}
