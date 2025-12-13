using Microsoft.EntityFrameworkCore;
using Ae.Poc.Identity.DbEntities;

namespace Ae.Poc.Identity.DbContexts;

public interface IIdentityDbContext
{
    DbSet<DbAccountIdentity> AccountIdentities { get; set; }

    DbSet<DbRefreshToken> RefreshTokens { get; set; }

    DbSet<DbAppClaim> AppClaims { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

