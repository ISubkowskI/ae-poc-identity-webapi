using Microsoft.Extensions.Logging;
using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.Interfaces;

namespace Ae.Poc.Identity.Services;

public sealed class IdentityMasterDataService(
   ILogger<IdentityMasterDataService> logger,
   IIdentityStorageService storage) : IIdentityMasterDataService
{
    private readonly ILogger<IdentityMasterDataService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IIdentityStorageService _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public async Task<IEnumerable<AppClaim>> GetClaimsAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default)
        => await _storage.GetAppClaimsAsync(skippedItems, numberOfItems, ct).ConfigureAwait(false);

    public async Task<AppClaim> GetClaimDetailsAsync(Guid id, CancellationToken ct = default)
        => await _storage.GetAppClaimDetailsAsync(id, ct).ConfigureAwait(false);

    public async Task<OperationResult<AppClaim>> AddClaimAsync(AppClaim claim, CancellationToken ct = default)
        => await _storage.AddClaimAsync(claim, ct).ConfigureAwait(false);

    public async Task<OperationResult<AppClaim>> DeleteClaimAsync(Guid id, CancellationToken ct = default)
        => await _storage.DeleteClaimAsync(id, ct).ConfigureAwait(false);

    public async Task<OperationResult<AppClaim>> UpdateClaimAsync(Guid id, AppClaim claim, CancellationToken ct)
        => await _storage.UpdateClaimAsync(id, claim, ct).ConfigureAwait(false);
}
