using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace Ae.Poc.Identity.Services;

public sealed class IdentityAccountsService(
    ILogger<IdentityAccountsService> logger,
    IIdentityStorageService storage) : IIdentityAccountsService
{
    private readonly ILogger<IdentityAccountsService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IIdentityStorageService _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public async Task<IEnumerable<AccountIdentity>> GetAccountIdentitiesAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default)
        => await _storage.GetAccountIdentitiesAsync(skippedItems, numberOfItems, ct).ConfigureAwait(false);

    public async Task<OperationResult<AccountRegistrationResult>> CreateAsync(AccountRegistration accountRegistration, CancellationToken ct = default)
    {
        // 1. Check if user exists
        var (exists, existingUser) = await _storage.TryGetAccountIdentityByEmailAsync(accountRegistration.Email, ct);
        if (exists)
        {
             return OperationResult<AccountRegistrationResult>.Failure("User already exists");
        }

        var newAccount = new AccountIdentity
        {
            Id = Guid.NewGuid(),
            EmailAddress = accountRegistration.Email,
            IsLocked = false
            // TenantId? AccountIdentity might not have it yet based on previous errors?
            // Checking AccountIdentity.cs earlier: Id, EmailAddress, PasswordHash, IsLocked.
        };

        var hasher = new PasswordHasher<AccountIdentity>();
        newAccount.PasswordHash = hasher.HashPassword(newAccount, accountRegistration.Password);

        var result = await _storage.CreateAccountAsync(newAccount, ct);
        return result;
    }


}
