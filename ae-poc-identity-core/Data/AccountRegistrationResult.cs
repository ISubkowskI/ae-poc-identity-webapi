namespace Ae.Poc.Identity.Data;

public sealed class AccountRegistrationResult
{
    public bool IsSuccess { get; set; } = false;

    public string InfoMessage { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public string EmailAddress { get; set; } = string.Empty;
}
