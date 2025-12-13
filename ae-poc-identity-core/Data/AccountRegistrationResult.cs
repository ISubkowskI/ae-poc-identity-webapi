namespace Ae.Poc.Identity.Data;

public sealed class AccountRegistrationResult
{
    public bool IsSuccess { get; set; } = false;

    public string InfoMessage { get; set; } = string.Empty;
}
