using Microsoft.EntityFrameworkCore;
using Ae.Poc.Identity.DbContexts;
using Ae.Poc.Identity.DbEntities;
using Ae.Poc.Identity.Interfaces;

namespace Ae.Poc.Identity.Repositories
{
    public sealed class IdentityRepository(IIdentityDbContext idbContext) : IIdentityRepository
    {
        public async Task<IEnumerable<DbAccountIdentity>> GetDbAccountIdentityRowsAsync(int skippedItems = 0, int numberOfRows = 50, CancellationToken ct = default)
            => await idbContext.AccountIdentities
            .OrderBy(_ => _.EmailAddress)
            .Skip(skippedItems)
            .Take(numberOfRows)
            .ToListAsync(ct);

        public async Task<IEnumerable<DbRefreshToken>> GetDbRefreshTokenRowsAsync(Guid accountIdentityId, int numberOfRows = 200, CancellationToken ct = default)
            => await idbContext.RefreshTokens
            .Where(_ => _.AccountIdentityId == accountIdentityId)
            .Take(numberOfRows)
            .ToListAsync(ct);

        public async Task<(bool success, DbAccountIdentity? dbAccountIdentity)> TryGetDbAccountIdentityByEmailAsync(string email, CancellationToken ct = default)
        {
            var dbAccountIdentity = await idbContext.AccountIdentities
                .FirstOrDefaultAsync(_ => _.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase), ct);

            return (dbAccountIdentity != null, dbAccountIdentity);
        }

        public async Task<IEnumerable<DbAppClaim>> GetDbAppClaimRowsAsync(int skippedItems = 0, int numberOfRows = 50, CancellationToken ct = default)
           => await idbContext.AppClaims
           .OrderBy(_ => _.Type)
           .ThenBy(_ => _.Value)
           .Skip(skippedItems)
           .Take(numberOfRows)
           .ToListAsync(ct);

        public async Task<DbAppClaim?> GetDbAppClaimAsync(Guid id, CancellationToken ct = default)
            => await idbContext.AppClaims.FirstOrDefaultAsync(_ => _.Id == id, ct);

        public async Task<DbAppClaim> AddDbAppClaimAsync(DbAppClaim dbAppClaim, CancellationToken ct = default)
        {
            var res = await idbContext.AppClaims.AddAsync(dbAppClaim, ct);
            await idbContext.SaveChangesAsync(ct);
            return res.Entity;
        }

        public async Task<bool> DeleteDbAppClaimAsync(DbAppClaim dbAppClaim, CancellationToken ct = default)
        {
            var entity = idbContext.AppClaims.Remove(dbAppClaim);
            var resDel = entity.State == EntityState.Deleted;
            var resSave = await idbContext.SaveChangesAsync(ct);
            return resDel && resSave == 1;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => idbContext.SaveChangesAsync(ct);

    }
}
