using System.Security.Claims;

namespace Ae.Poc.Identity.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateAccessToken(ClaimsIdentity claimsIdentity);
    string GenerateRefreshToken();
}
