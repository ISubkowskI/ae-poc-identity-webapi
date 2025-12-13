namespace Ae.Poc.Identity.Dtos;

public sealed record AccountRegistrationOutgoingDto
{
    public bool IsSuccess { get; init; } = false;

    public string InfoMessage { get; init; } = string.Empty;
}
