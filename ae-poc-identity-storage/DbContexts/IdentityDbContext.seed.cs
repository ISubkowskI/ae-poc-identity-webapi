using Ae.Poc.Identity.DbEntities;
using Ae.Poc.Identity.Security;
using Ae.Poc.Identity.Utils;
using System.Globalization;
using System.Security.Claims;

namespace Ae.Poc.Identity.DbContexts
{
    public partial class IdentityDbContext
    {
        public void Seed()
        {
            bool modifiedAppClaims = SeedAppClaims();
            bool modifiedAccountIdentities = SeedAccountIdentities();

            if (modifiedAppClaims || modifiedAccountIdentities)
            {
                SaveChanges();
            }
        }
        public bool SeedAccountIdentities()
        {
            if (AccountIdentities.Any())
                return false;

            var accountInfo = new DbAccountIdentity
            {
                EmailAddress = "info@softaren.com",
                PasswordHash = "hashed-password-1",
                DisplayName = "Info",
                Description = "Demo User Info",
                IsLocked = false,
                ClaimsJson = "{}",
                CreatedAtUtc = DateTimeOffset.UtcNow,
                EmploymentDate = DateOnly.FromDateTime(DateTimeOffset.Parse("2024-03-01").DateTime),
                EmploymentExpiredDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddYears(1).DateTime),
                LastLoginUtc = DateTimeOffset.MinValue,
                LastPasswordChangeUtc = DateTimeOffset.UtcNow,
                PasswordExpiredOnUtc = DateTimeOffset.UtcNow.AddMonths(3),
                EmailVerifiedOnUtc = DateTimeOffset.MinValue,
            };
            var claimsInfo = new List<Claim>
                                {
                                    //new (ClaimTypes.Name, string.IsNullOrWhiteSpace(accountInfo.DisplayName) ? accountInfo.EmailAddress : accountInfo.DisplayName),
                                    //new (ClaimTypes.NameIdentifier, $"{accountInfo.Id}"),
                                    //new (ClaimTypes.Email, accountInfo.EmailAddress),
                                    new (ClaimTypes.Role, AppRoles.Administrator),
                                    new (ClaimTypes.Role, AppRoles.Demo),
                                    new (AppClaimTypes.Department, "HR"),
                                    new (AppClaimTypes.Admin, "true"),
                                    new (AppClaimTypes.Manager, "true"),
                                    //new (AppClaimTypes.EmploymentDate, accountIdentity.ToStringEmploymentDate()),
                                };
            accountInfo.ClaimsJson = ClaimUtils.SerializeToJson(claimsInfo);

            var accountNotifications = new DbAccountIdentity
            {
                EmailAddress = "notifications@softaren.com",
                PasswordHash = "hashed-password-1",
                DisplayName = "Notifications",
                Description = "Demo User Notifications",
                IsLocked = false,
                ClaimsJson = "{}",
                CreatedAtUtc = DateTimeOffset.UtcNow,
                EmploymentDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.DateTime),
                EmploymentExpiredDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddYears(1).DateTime),
                LastLoginUtc = DateTimeOffset.MinValue,
                LastPasswordChangeUtc = DateTimeOffset.UtcNow,
                PasswordExpiredOnUtc = DateTimeOffset.UtcNow.AddMonths(3),
                EmailVerifiedOnUtc = DateTimeOffset.MinValue,
            };
            var claimsNotifications = new List<Claim>
                                {
                                    //new (ClaimTypes.Name, string.IsNullOrWhiteSpace(accountInfo.DisplayName) ? accountInfo.EmailAddress : accountInfo.DisplayName),
                                    //new (ClaimTypes.NameIdentifier, $"{accountInfo.Id}"),
                                    //new (ClaimTypes.Email, accountInfo.EmailAddress),
                                    new (ClaimTypes.Role, AppRoles.Administrator),
                                    new (ClaimTypes.Role, AppRoles.Demo),
                                    new (AppClaimTypes.Department, "HR"),
                                    //new (AppClaimTypes.Admin, "true"),
                                    new (AppClaimTypes.Manager, "true"),
                                    //new (AppClaimTypes.EmploymentDate, accountIdentity.ToStringEmploymentDate()),
                                };
            accountNotifications.ClaimsJson = ClaimUtils.SerializeToJson(claimsNotifications);

            var account1 = new DbAccountIdentity
            {
                EmailAddress = "user1@example.com",
                PasswordHash = "hashed-password-1",
                DisplayName = "User One",
                Description = "Test First user in the system",
                IsLocked = false,
                ClaimsJson = ClaimUtils.SerializeToJson([]),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                EmploymentDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.DateTime),
                EmploymentExpiredDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddYears(1).DateTime),
                LastLoginUtc = DateTimeOffset.UtcNow,
                LastPasswordChangeUtc = DateTimeOffset.UtcNow,
                PasswordExpiredOnUtc = DateTimeOffset.UtcNow.AddMonths(3),
                EmailVerifiedOnUtc = DateTimeOffset.MinValue,
            };

            var account2 = new DbAccountIdentity
            {
                EmailAddress = "user2@example.com",
                PasswordHash = "hashed-password-2",
                DisplayName = "User Two",
                IsLocked = true,
                ClaimsJson = ClaimUtils.SerializeToJson([]),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                EmploymentDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.DateTime),
                EmploymentExpiredDate = DateOnly.FromDateTime(DateTimeOffset.UtcNow.AddYears(1).DateTime),
                LastLoginUtc = DateTimeOffset.UtcNow,
                LastPasswordChangeUtc = DateTimeOffset.UtcNow,
                PasswordExpiredOnUtc = DateTimeOffset.UtcNow.AddMonths(3),
                EmailVerifiedOnUtc = DateTimeOffset.MinValue,
            };

            AccountIdentities.AddRange(accountInfo, accountNotifications, account1, account2);

            RefreshTokens.AddRange(
                new DbRefreshToken
                {
                    Id = Guid.NewGuid(),
                    AccountIdentityId = account1.Id,
                    TokenHash = "hash1",
                    CreatedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    CreatedByIp = "127.0.0.1"
                },
                new DbRefreshToken
                {
                    Id = Guid.NewGuid(),
                    AccountIdentityId = account2.Id,
                    TokenHash = "hash2",
                    CreatedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    CreatedByIp = "127.0.0.2"
                });
            return true;
        }

        private bool SeedAppClaims()
        {
            if (AppClaims.Any())
                return false;

            var adminClaim = new Claim(AppClaimTypes.Admin, false.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean);
            var managerClaim = new Claim(AppClaimTypes.Manager, false.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean);
            
            var accountBalanceClaim = new Claim(AppClaimTypes.AccountBalance, 1500.00d.ToString("R", CultureInfo.InvariantCulture), ClaimValueTypes.Double);
            accountBalanceClaim.Properties.Add("currency", "DEMOPOINT");
            accountBalanceClaim.Properties.Add("description", "Account balance in DEMOPOINT");

            AppClaims.AddRange(
                new DbAppClaim
                {
                    Type = ClaimTypes.Role,
                    Value = "Administrator",
                    ValueType = ClaimValueTypes.String,
                    DisplayText = "Administrator role",
                    PropertiesJson = "{}",
                    Description = "Administrator role"
                },
                new DbAppClaim
                {
                    Type = ClaimTypes.Role,
                    Value = "Demo",
                    ValueType = ClaimValueTypes.String,
                    DisplayText = "Demo role",
                    PropertiesJson = "{}",
                    Description = "Demo role"
                },
                new DbAppClaim
                {
                    Type = ClaimTypes.DateOfBirth,
                    Value = (new DateTime(1991, 5, 15)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ValueType = ClaimValueTypes.Date,
                    DisplayText = "Date Of Birth",
                    PropertiesJson = "{}",
                    Description = "Date Of Birth"
                },
                new DbAppClaim
                {
                    Type = AppClaimTypes.UserLevel,
                    Value = 0.ToString(CultureInfo.InvariantCulture),
                    ValueType = ClaimValueTypes.Integer,
                    DisplayText = "User Level 0",
                    PropertiesJson = "{}",
                    Description = "User Level 0"
                },
                new DbAppClaim
                {
                    Type = AppClaimTypes.UserLevel,
                    Value = 1.ToString(CultureInfo.InvariantCulture),
                    ValueType = ClaimValueTypes.Integer,
                    DisplayText = "User Level 1",
                    PropertiesJson = "{}",
                    Description = "User Level 1"
                },
                new DbAppClaim
                {
                    Type = AppClaimTypes.UserLevel,
                    Value = 5.ToString(CultureInfo.InvariantCulture),
                    ValueType = ClaimValueTypes.Integer,
                    DisplayText = "User Level 5",
                    PropertiesJson = "{}",
                    Description = "User Level 5"
                },
                // adminClaim
                new DbAppClaim
                {
                    Type = adminClaim.Type,
                    Value = adminClaim.Value,
                    ValueType = adminClaim.ValueType,
                    DisplayText = "Is Admin",
                    PropertiesJson = "{}",
                    Description = "Is Admin"
                },
                // managerClaim
                new DbAppClaim
                {
                    Type = managerClaim.Type,
                    Value = managerClaim.Value,
                    ValueType = managerClaim.ValueType,
                    DisplayText = "Is Manager",
                    PropertiesJson = "{}",
                    Description = "Is Manager"
                },
                // accountBalanceClaim
                new DbAppClaim
                {
                    Type = accountBalanceClaim.Type,
                    Value = accountBalanceClaim.Value,
                    ValueType = accountBalanceClaim.ValueType,
                    DisplayText = "Account Balance",
                    PropertiesJson = ClaimUtils.SerializePropertiesToJson(accountBalanceClaim.Properties),
                    Description = "Account Balance"
                },
                new DbAppClaim
                {
                    Type = AppClaimTypes.Department,
                    Value = "HR",
                    ValueType = ClaimValueTypes.String,
                    DisplayText = "Human Resources Department",
                    PropertiesJson = "{}",
                    Description = "Human Resources Department"
                },
                new DbAppClaim
                {
                    Type = AppClaimTypes.Department,
                    Value = "Engineering",
                    ValueType = ClaimValueTypes.String,
                    DisplayText = "Engineering department",
                    PropertiesJson = "{}",
                    Description = "Engineering department"
                });
            return true;
        }
    }
}
