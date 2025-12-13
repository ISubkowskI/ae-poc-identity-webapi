using Ae.Poc.Identity.Data;

namespace Ae.Poc.Identity.Services;

public interface IIdentityMasterDataService
{
    Task<IEnumerable<AppClaim>> GetClaimsAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default);
    Task<AppClaim> GetClaimDetailsAsync(Guid id, CancellationToken ct = default);
    Task<OperationResult<AppClaim>> AddClaimAsync(AppClaim claim, CancellationToken ct = default);
    Task<OperationResult<AppClaim>> DeleteClaimAsync(Guid id, CancellationToken ct = default);
    Task<OperationResult<AppClaim>> UpdateClaimAsync(Guid id, AppClaim claim, CancellationToken ct);
}
