using System.ComponentModel.DataAnnotations;

namespace Ae.Poc.Identity.Dtos;

public sealed record AccountRegistrationIncomingDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string? Description { get; init; }
}
