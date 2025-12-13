using Ae.Poc.Identity.Authentication;

namespace Ae.Poc.Identity.Services;

public interface IIdentityService
{
    Task<ClientCredentialsResult> TryVerifyClientCredentialAsync(string email, string password, CancellationToken ct = default);
}
