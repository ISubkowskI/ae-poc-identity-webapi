using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Ae.Poc.Identity.Profiles;
using Ae.Poc.Identity.Services;
using Ae.Poc.Identity.UseCases;
using Ae.Poc.Identity.Interfaces;
using Ae.Poc.Identity.Settings;
using Ae.Poc.Identity.Repositories;

namespace Ae.Poc.Identity.Extensions;

/// <summary>
/// Extension methods for configuring and registering application services
/// </summary>
public static class WebApiExtensions
{
    /// <summary>
    /// Registers configuration options from appsettings.json into the DI container
    /// </summary>
    /// <param name="services">The service collection to add configurations to</param>
    /// <param name="config">The configuration root containing the settings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services,
         IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(config);

        services
            .Configure<IdentityApiOptions>(config.GetSection(IdentityApiOptions.App))
            .Configure<HealthOptions>(config.GetSection(HealthOptions.Health))
            .Configure<IdentityStorageOptions>(config.GetSection(IdentityStorageOptions.IdentityStorage))
            .Configure<IdentityTokenOptions>(config.GetSection(IdentityTokenOptions.IdentityToken));

        return services;
    }

    /// <summary>
    /// Configures and registers an in-memory SQLite database context
    /// </summary>
    /// <typeparam name="TContextService">The DB context service interface</typeparam>
    /// <typeparam name="TContextImplementation">The concrete DB context implementation</typeparam>
    /// <param name="services">The service collection to add the DB context to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSqliteDbContext<TContextService, TContextImplementation>(
        this IServiceCollection services)
        where TContextImplementation : DbContext, TContextService
    {
        ArgumentNullException.ThrowIfNull(services);

        // Initialize SQLite native library
        SQLitePCL.Batteries_V2.Init();

        // Create and open an in-memory SQLite connection
        // Note: Connection must remain open for the lifetime of the application
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        services.AddSingleton(connection);

        // Register the DB context with the SQLite connection
        services.AddDbContext<TContextService, TContextImplementation>((sp, options) =>
        {
            var conn = sp.GetRequiredService<SqliteConnection>();
            options.UseSqlite(conn);
        });

        return services;
    }

    /// <summary>
    /// Registers AutoMapper profiles for DTO and storage mappings
    /// </summary>
    /// <param name="services">The service collection to add mapper profiles to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAppMapper(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAutoMapper(m =>
        {
            m.AddProfile<DtoProfile>();             // Maps DTOs to/from domain models
            m.AddProfile<IdentityStorageProfile>(); // Maps storage models to/from domain models
        });
        return services;
    }

    /// <summary>
    /// Registers application services in the DI container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register repositories and services with their implementations
        services.AddTransient<IIdentityRepository, IdentityRepository>();
        services.AddTransient<IIdentityStorageService, IdentityStorageService>();

        services.AddTransient<IIdentityMasterDataService, IdentityMasterDataService>();
        services.AddTransient<IIdentityAccountsService, IdentityAccountsService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }
}
