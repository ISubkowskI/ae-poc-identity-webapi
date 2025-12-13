using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ae.Poc.Identity.Settings;
using Ae.Poc.Identity.Data;

namespace Ae.Poc.Identity.Services;

public sealed class TokenService(ILogger<TokenService> logger, IOptions<IdentityTokenOptions> identityTokenOptions) : ITokenService
{
    private readonly ILogger<TokenService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IdentityTokenOptions _tokenOptions = identityTokenOptions?.Value ?? throw new ArgumentNullException(nameof(identityTokenOptions));

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

        //var tokenDescriptor = new SecurityTokenDescriptor
        //{
        //    Subject = new ClaimsIdentity(claims),
        //}

        var tokeOptions = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenOptions.AccessExpiresInMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }
    public string GenerateAccessToken(ClaimsIdentity claimsIdentity)
    {
        if (claimsIdentity == null)
        {
            throw new ArgumentNullException(nameof(claimsIdentity));
        }
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessExpiresInMinutes),
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //public RefreshToken GenerateRefreshToken()
    //{
    //    int expiresInDays = 5;
    //    //var randomNumber = new byte[32];
    //    //using (var rng = RandomNumberGenerator.Create())
    //    //{
    //    //    rng.GetBytes(randomNumber);
    //    //    return Convert.ToBase64String(randomNumber);
    //    //}

    //    RefreshToken refreshToken = new()
    //    {
    //        Id = Guid.NewGuid(),
    //        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
    //        CreatedUtc = DateTimeOffset.UtcNow,
    //        ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(expiresInDays)
    //    };
    //    return refreshToken;
    //}
    

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            //ValidateLifetime = false, //here we are saying that we don't care about the token's expiration
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        return principal;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    //public RefreshToken GenerateRefreshToken111()
    //{
    //    int expiresInDays = 5;
    //    //var randomNumber = new byte[32];
    //    //using (var rng = RandomNumberGenerator.Create())
    //    //{
    //    //    rng.GetBytes(randomNumber);
    //    //    return Convert.ToBase64String(randomNumber);
    //    //}

    //    RefreshToken refreshToken = new()
    //    {
    //        Id = Guid.NewGuid(),
    //        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
    //        CreatedUtc = DateTimeOffset.UtcNow,
    //        ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(expiresInDays)
    //    };
    //    return refreshToken;
    //}

    static string Encrypt(string plaintext, byte[] masterKey)
    {
        const int iVSize = 16;
        try
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = masterKey;
            aes.IV = RandomNumberGenerator.GetBytes(aes.BlockSize / 8); // AES block size is 128 bits (16 bytes)
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plaintext);
                }
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Encryption failed", ex);
        }
    }

    static string Decrypt(string ciphertext, byte[] masterKey)
    {
        const int iVSize = 16;

        try
        {
            var cipherData = Convert.FromBase64String(ciphertext);
            if (cipherData.Length < iVSize)
            {
                throw new ArgumentException("Ciphertext is too short", nameof(ciphertext));
            }

            var iv = new byte[iVSize];
            var encryptedData = new byte[cipherData.Length - iv.Length];
            Buffer.BlockCopy(cipherData, 0, iv, 0, iVSize);
            Buffer.BlockCopy(cipherData, iVSize, encryptedData, 0, encryptedData.Length);
            //Array.Copy(cipherData, iv, iv.Length);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = masterKey;
            aes.IV = iv;

            using var ms = new MemoryStream(encryptedData);
            using var decryptor = aes.CreateDecryptor(); // aes.CreateDecryptor(aes.Key, aes.IV);
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Decryption failed", ex);
        }
    }


}
