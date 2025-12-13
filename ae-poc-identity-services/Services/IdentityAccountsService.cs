using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ae.Poc.Identity.Services;

public sealed class IdentityAccountsService(
    ILogger<IdentityAccountsService> logger,
    IIdentityStorageService storage) : IIdentityAccountsService
{
    private readonly ILogger<IdentityAccountsService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IIdentityStorageService _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public async Task<IEnumerable<AccountIdentity>> GetAccountIdentitiesAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default)
        => await _storage.GetAccountIdentitiesAsync(skippedItems, numberOfItems, ct).ConfigureAwait(false);

    public async Task<AccountRegistrationResult> CreateAsync(AccountRegistration accountRegistration, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }


}
