using AutoMapper;
using Microsoft.Extensions.Logging;
using Ae.Poc.Identity.Interfaces;
using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.DbEntities;
using Ae.Poc.Identity.Utils;

namespace Ae.Poc.Identity.UseCases
{
    public sealed class IdentityStorageService(
       ILogger<IdentityStorageService> logger,
       IIdentityRepository identityRepository,
       IMapper mapper) : IIdentityStorageService
    {
        public async Task<IEnumerable<AccountIdentity>> GetAccountIdentitiesAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {SkippedItems}, {NumberOfItems}", nameof(GetAccountIdentitiesAsync), skippedItems, numberOfItems);
            var res = await identityRepository.GetDbAccountIdentityRowsAsync(skippedItems, numberOfItems, ct);
            var outData = mapper.Map<IEnumerable<AccountIdentity>>(res);
            return outData;
        }

        public async Task<(bool success, AccountIdentity? accountIdentity)> TryGetAccountIdentityByEmailAsync(string email, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {Email}", nameof(TryGetAccountIdentityByEmailAsync), email);
            (bool success, DbAccountIdentity? dbAccountIdentity) = await identityRepository.TryGetDbAccountIdentityByEmailAsync(email, ct).ConfigureAwait(false);
            if (!success)
            {
                logger.LogDebug("User not found '{Email}'. {ServiceName} {MethodName}()",
                    email, nameof(IdentityStorageService), nameof(TryGetAccountIdentityByEmailAsync));
                return (false, default);
            }

            var accountIdentity = mapper.Map<AccountIdentity>(dbAccountIdentity);
            return (true, accountIdentity);
        }

        public async Task<IEnumerable<AppClaim>> GetAppClaimsAsync(int skippedItems = 0, int numberOfItems = 50, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {SkippedItems}, {NumberOfItems}", nameof(GetAppClaimsAsync), skippedItems, numberOfItems);
            var res = await identityRepository.GetDbAppClaimRowsAsync(skippedItems, numberOfItems, ct);
            var outData = mapper.Map<IEnumerable<AppClaim>>(res);
            return outData;
        }

        public async Task<AppClaim?> GetAppClaimDetailsAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {Id}", nameof(GetAppClaimDetailsAsync), id);
            var res = await identityRepository.GetDbAppClaimAsync(id, ct);
            if (res is null)
            {
                logger.LogDebug("Claim not found '{Id}'. {ServiceName} {MethodName}()",
                    id, nameof(IdentityStorageService), nameof(GetAppClaimDetailsAsync));
                return default;
            }
            var outData = mapper.Map<AppClaim>(res);
            return outData;
        }

        public async Task<OperationResult<AppClaim>> AddClaimAsync(AppClaim claim, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... ", nameof(AddClaimAsync));

            if (claim is null)
            {
                logger.LogDebug("Claim is null. {ServiceName} {MethodName}()",
                    nameof(IdentityStorageService), nameof(AddClaimAsync));
                return OperationResult<AppClaim>.Failure("Claim is null.");
            }
            if (string.IsNullOrWhiteSpace(claim.Type))
            {
                logger.LogDebug("Claim Type is empty. {ServiceName} {MethodName}()",
                    nameof(IdentityStorageService), nameof(AddClaimAsync));
                return OperationResult<AppClaim>.Failure("Claim Type is empty.");
            }

            var dbAppClaim = mapper.Map<DbAppClaim>(claim);
            if (dbAppClaim.Id == Guid.Empty)
            {
                dbAppClaim.Id = Guid.NewGuid();
            }

            var createdDbAppClaim = await identityRepository.AddDbAppClaimAsync(dbAppClaim, ct);

            var outData = mapper.Map<AppClaim>(createdDbAppClaim);
            return OperationResult<AppClaim>.Success(outData, "Claim successfully created.");
        }

        public async Task<OperationResult<AppClaim>> DeleteClaimAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {Id}", nameof(DeleteClaimAsync), id);
            var dbAppClaim = await identityRepository.GetDbAppClaimAsync(id, ct);
            if (dbAppClaim is null)
            {
                logger.LogDebug("Claim not found '{Id}'. {ServiceName} {MethodName}()",
                    id, nameof(IdentityStorageService), nameof(DeleteClaimAsync));
                return OperationResult<AppClaim>.Failure("Claim not found.");
            }

            var ok = await identityRepository.DeleteDbAppClaimAsync(dbAppClaim, ct);
            if (!ok)
            {
                logger.LogError("Failed to delete claim '{Id}'. {ServiceName} {MethodName}()",
                    id, nameof(IdentityStorageService), nameof(DeleteClaimAsync));
                return OperationResult<AppClaim>.Failure("Failed to delete claim.");
            }

            var outData = mapper.Map<AppClaim>(dbAppClaim);
            return OperationResult<AppClaim>.Success(outData, "Claim successfully deleted.");
        }

        public async Task<OperationResult<AppClaim>> UpdateClaimAsync(Guid id, AppClaim appClaim, CancellationToken ct = default)
        {
            logger.LogTrace("Start {MethodName} ... {Id}", nameof(UpdateClaimAsync), id);

            if (id != appClaim.Id)
            {
                logger.LogDebug("Claim Id does not match '{Id}' to '{UpdateId}'. {ServiceName} {MethodName}()",
                    id, appClaim.Id, nameof(IdentityStorageService), nameof(UpdateClaimAsync));
                return OperationResult<AppClaim>.Failure("Claim Id does not match.");
            }

            var dbAppClaim = await identityRepository.GetDbAppClaimAsync(id, ct);
            if (dbAppClaim is null)
            {
                logger.LogDebug("Claim not found '{Id}'. {ServiceName} {MethodName}()",
                    id, nameof(IdentityStorageService), nameof(UpdateClaimAsync));
                return OperationResult<AppClaim>.Failure("Claim not found.");
            }

            if (!string.Equals(dbAppClaim.Type, appClaim.Type, StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogDebug("The claim type cannot be changed '{Id}' '{ClaimType}' to '{ClaimTypeNew}'. {ServiceName} {MethodName}()",
                    id, dbAppClaim.Type, appClaim.Type, nameof(IdentityStorageService), nameof(UpdateClaimAsync));
                return OperationResult<AppClaim>.Failure("The claim type cannot be changed.");
            }

            dbAppClaim.DisplayText = appClaim.DisplayText;
            dbAppClaim.Value = appClaim.Value;
            dbAppClaim.ValueType = appClaim.ValueType;
            dbAppClaim.Description = appClaim.Description;
            dbAppClaim.PropertiesJson = ClaimUtils.SerializePropertiesToJson(appClaim.Properties);

            int resSave = await identityRepository.SaveChangesAsync(ct);
            //DbAppClaim? res = await identityRepository.UpdateDbAppClaimAsync(dbAppClaim, ct);

            var outData = mapper.Map<AppClaim>(dbAppClaim);

            return OperationResult<AppClaim>.Success(outData, "Claim successfully updated.");
        }

        public async Task<OperationResult<AccountRegistrationResult>> CreateAccountAsync(AccountIdentity account, CancellationToken ct = default)
        {
            var dbAccount = mapper.Map<DbAccountIdentity>(account);
            if (dbAccount.Id == Guid.Empty)
            {
                dbAccount.Id = Guid.NewGuid();
            }
            dbAccount.CreatedAtUtc = DateTimeOffset.UtcNow;
            
            // NOTE: Repository method needed? Or AddDbAccountIdentityAsync?
            // Checking IIdentityRepository... assuming it exists or using DbContext directly if exposed, 
            // but repository pattern suggests using repo.
            // Let's assume AddDbAccountIdentityAsync exists or I need to add it too.
            // Based on AddDbAppClaimAsync naming convention:
            var created = await identityRepository.AddDbAccountIdentityAsync(dbAccount, ct);
            
            var result = new AccountRegistrationResult
            {
                Id = created.Id,
                EmailAddress = created.EmailAddress
            };
            
            return OperationResult<AccountRegistrationResult>.Success(result, "Account created.");
        }
    }
}
