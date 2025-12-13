namespace Ae.Poc.Identity.Dtos;

public sealed record AppClaimIncomingDto
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string ValueType { get; init; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public IDictionary<string, string>? Properties { get; init; }
    public string? Description { get; init; }
}
