using Ae.Poc.Identity.Data;

namespace Ae.Poc.Identity.Interfaces;

public interface IIdentityStorageService
{
    Task<IEnumerable<AccountIdentity>> GetAccountIdentitiesAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default);

    Task<(bool success, AccountIdentity? accountIdentity)> TryGetAccountIdentityByEmailAsync(string email, CancellationToken ct = default);

    Task<IEnumerable<AppClaim>> GetAppClaimsAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default);

    Task<AppClaim?> GetAppClaimDetailsAsync(Guid id, CancellationToken ct = default);

    Task<OperationResult<AppClaim>> AddClaimAsync(AppClaim claim, CancellationToken ct = default);

    Task<OperationResult<AppClaim>> DeleteClaimAsync(Guid id, CancellationToken ct = default);

    Task<OperationResult<AppClaim>> UpdateClaimAsync(Guid id, AppClaim claim, CancellationToken ct = default);
}
