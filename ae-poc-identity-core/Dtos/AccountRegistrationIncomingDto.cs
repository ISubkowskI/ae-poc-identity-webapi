namespace Ae.Poc.Identity.Dtos;

public sealed record AccountRegistrationIncomingDto
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string? Description { get; init; }
}
