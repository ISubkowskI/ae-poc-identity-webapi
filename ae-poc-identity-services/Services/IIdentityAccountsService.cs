using Ae.Poc.Identity.Data;

namespace Ae.Poc.Identity.Services;

public interface IIdentityAccountsService
{
    Task<IEnumerable<AccountIdentity>> GetAccountIdentitiesAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default);
    Task<AccountRegistrationResult> CreateAsync(AccountRegistration accountRegistration, CancellationToken ct = default);
}
