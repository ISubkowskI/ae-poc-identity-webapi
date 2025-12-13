using Ae.Poc.Identity.DbEntities;

namespace Ae.Poc.Identity.Interfaces;

public interface IIdentityRepository
{
    Task<IEnumerable<DbAccountIdentity>> GetDbAccountIdentityRowsAsync(int skippedItems = 0, int numberOfRows = 50, CancellationToken ct = default);

    Task<IEnumerable<DbRefreshToken>> GetDbRefreshTokenRowsAsync(Guid accountIdentityId, int numberOfRows = 200, CancellationToken ct = default);

    Task<(bool success, DbAccountIdentity? dbAccountIdentity)> TryGetDbAccountIdentityByEmailAsync(string email, CancellationToken ct = default);

    Task<IEnumerable<DbAppClaim>> GetDbAppClaimRowsAsync(int skippedItems = 0, int numberOfRows = 50, CancellationToken ct = default);

    Task<DbAppClaim?> GetDbAppClaimAsync(Guid id, CancellationToken ct = default);

    Task<DbAppClaim> AddDbAppClaimAsync(DbAppClaim dbAppClaim, CancellationToken ct = default);

    Task<bool> DeleteDbAppClaimAsync(DbAppClaim dbAppClaim, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<DbAccountIdentity> AddDbAccountIdentityAsync(DbAccountIdentity dbAccount, CancellationToken ct = default);

    //Task<DbAppClaim?> UpdateDbAppClaimAsync(DbAppClaim dbAppClaim, CancellationToken ct = default);
}
