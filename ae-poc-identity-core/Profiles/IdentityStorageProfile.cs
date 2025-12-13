using AutoMapper;
using Ae.Poc.Identity.Data;
using Ae.Poc.Identity.DbEntities;
using Ae.Poc.Identity.Utils;

namespace Ae.Poc.Identity.Profiles;

public sealed class IdentityStorageProfile : Profile
{
    public IdentityStorageProfile()
    {
        CreateMap<DbAccountIdentity, AccountIdentity>()
            .ConvertUsing((source, destination, context) =>
            { 
                return new AccountIdentity
                {
                    Id = source.Id,
                    EmailAddress = source.EmailAddress,
                    PasswordHash = source.PasswordHash,
                    DisplayName = source.DisplayName,
                    Description = source.Description ?? string.Empty,
                    IsLocked = source.IsLocked,
                    Claims = ClaimUtils.DeserializeFromJson(source.ClaimsJson),
                    CreatedAtUtc = source.CreatedAtUtc,
                    EmploymentDate = source.EmploymentDate,
                    EmploymentExpiredDate = source.EmploymentExpiredDate,
                    LastLoginUtc = source.LastLoginUtc,
                    LastPasswordChangeUtc = source.LastPasswordChangeUtc,
                    PasswordExpiredOnUtc = source.PasswordExpiredOnUtc,
                    EmailVerifiedOnUtc = source.EmailVerifiedOnUtc
                };
            });
        CreateMap<AccountIdentity, DbAccountIdentity>()
            .ConvertUsing((source, destination, context) =>
            {
                return new DbAccountIdentity
                {
                    Id = (source.Id == Guid.Empty) ? Guid.NewGuid() : source.Id,
                    EmailAddress = source.EmailAddress,
                    PasswordHash = source.PasswordHash,
                    DisplayName = source.DisplayName,
                    Description = source.Description,
                    IsLocked = source.IsLocked,
                    ClaimsJson = ClaimUtils.SerializeToJson(source.Claims),
                    CreatedAtUtc = source.CreatedAtUtc,
                    EmploymentDate = source.EmploymentDate,
                    EmploymentExpiredDate = source.EmploymentExpiredDate,
                    LastLoginUtc = source.LastLoginUtc,
                    LastPasswordChangeUtc = source.LastPasswordChangeUtc,
                    PasswordExpiredOnUtc = source.PasswordExpiredOnUtc,
                    EmailVerifiedOnUtc = source.EmailVerifiedOnUtc
                };
            });

        CreateMap<DbRefreshToken, RefreshToken>();
        CreateMap<RefreshToken, DbRefreshToken>();

        CreateMap<DbAppClaim, AppClaim>()
            .ConvertUsing((source, destination, context) =>
            {
                return new AppClaim
                {
                    Id = source.Id,
                    Type = source.Type,
                    Value = source.Value,
                    ValueType = source.ValueType,
                    DisplayText = source.DisplayText,
                    Properties = ClaimUtils.DeserializePropertiesFromJson(source.PropertiesJson),
                    Description = source.Description ?? string.Empty,
                };
            });
        CreateMap<AppClaim, DbAppClaim>()
            .ConvertUsing((source, destination, context) =>
            {
                return new DbAppClaim
                {
                    Id = source.Id,
                    Type = source.Type,
                    Value = source.Value,
                    ValueType = source.ValueType,
                    DisplayText = source.DisplayText,
                    PropertiesJson = ClaimUtils.SerializePropertiesToJson(source.Properties),
                    Description = source.Description ?? string.Empty,
                };
            });
    }
}
