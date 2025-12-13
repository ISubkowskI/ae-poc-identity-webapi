using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ae.Poc.Identity.DbEntities;
using System.Globalization;

namespace Ae.Poc.Identity.DbContexts
{
    public partial class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options), IIdentityDbContext
    {
        public DbSet<DbAccountIdentity> AccountIdentities { get; set; }
        public DbSet<DbRefreshToken> RefreshTokens { get; set; }
        public DbSet<DbAppClaim> AppClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Support for DateOnly mapping in SQLite
            var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                s => DateOnly.Parse(s));

            modelBuilder.Entity<DbAccountIdentity>(entity =>
            {
                // Primary Key
                entity.HasKey(a => a.Id);

                // Email Address
                entity.Property(a => a.EmailAddress)
                      .IsRequired()
                      .HasMaxLength(256); // Enforce a reasonable max length
                entity.HasIndex(a => a.EmailAddress)
                      .IsUnique()
                      .HasDatabaseName("IX_AccountIdentities_EmailAddress");

                // Password Hash
                entity.Property(a => a.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(512); // Enforce a max length for security
                // Display Name
                entity.Property(a => a.DisplayName)
                      .IsRequired()
                      .HasMaxLength(100); // Limit display name length
                // Description
                entity.Property(a => a.Description)
                      .HasMaxLength(500) // Optional, with a max length
                      .IsRequired(false);
                // IsLocked
                entity.Property(a => a.IsLocked)
                      .IsRequired()
                      .HasDefaultValue(false); // Default to false

                // Claims JSON
                entity.Property(c => c.ClaimsJson)
                      .HasColumnType("TEXT")
                      .IsRequired(); // Ensure it's not null

                // CreatedAtUtc
                entity.Property(a => a.CreatedAtUtc)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default to current timestamp

                // Employment Dates
                entity.Property(a => a.EmploymentDate)
                      .HasConversion(dateOnlyConverter)
                      .IsRequired()
                      .HasDefaultValue(DateOnly.MinValue);
                entity.Property(a => a.EmploymentExpiredDate)
                      .HasConversion(dateOnlyConverter)
                      .IsRequired()
                      .HasDefaultValue(DateOnly.MinValue);

                entity.Property(a => a.LastLoginUtc)
                      .IsRequired()
                      .HasDefaultValue(DateTimeOffset.MinValue);
                entity.Property(a => a.LastPasswordChangeUtc)
                      .IsRequired()
                      .HasDefaultValue(DateTimeOffset.MinValue);
                entity.Property(a => a.PasswordExpiredOnUtc)
                      .IsRequired()
                      .HasDefaultValue(DateTimeOffset.MinValue);
                entity.Property(a => a.EmailVerifiedOnUtc)
                      .IsRequired()
                      .HasDefaultValue(DateTimeOffset.MinValue);
            });

            modelBuilder.Entity<DbRefreshToken>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.AccountIdentityId).IsRequired();

                entity.Property(t => t.TokenHash).IsRequired();
                entity.Property(t => t.ExpiresUtc).IsRequired();
                entity.Property(t => t.CreatedUtc).IsRequired();
                entity.Property(t => t.CreatedByIp).IsRequired();

                entity.Property(t => t.RevokedUtc);
                entity.Property(t => t.RevokedByIp).HasMaxLength(100);
                entity.Property(t => t.ReplacedByTokenHash).HasMaxLength(512);

                entity.HasIndex(t => new { t.AccountIdentityId, t.CreatedUtc }); // For cleanup/rotation

                entity.HasOne<DbAccountIdentity>()
                      .WithMany()
                      .HasForeignKey(t => t.AccountIdentityId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade deletes of refresh tokens
            });

            modelBuilder.Entity<DbAppClaim>(entity =>
            {
                // Primary Key
                entity.HasKey(a => a.Id);
                entity.Property(c => c.Type).IsRequired();
                entity.Property(c => c.Value).IsRequired();
                entity.Property(c => c.ValueType).IsRequired();
                entity.Property(c => c.DisplayText).IsRequired();
                entity.Property(c => c.PropertiesJson).HasColumnType("TEXT");
                entity.Property(c => c.Description).HasMaxLength(500);

                entity.HasIndex(t => new { t.Type, t.Value })
                      .IsUnique()
                      .HasDatabaseName("IX_AppClaims_Type_Value");
            });
        }
    }
}
