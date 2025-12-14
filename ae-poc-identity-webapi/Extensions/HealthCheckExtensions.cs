using Ae.Poc.Identity.DbContexts;
using Ae.Poc.Identity.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ae.Poc.Identity.Extensions;

/// <summary>
/// Extension methods for configuring and managing application health checks.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Registers the necessary health check services, including EF Core Context checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddIdentityHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<IdentityDbContext>();

        return services;
    }

    /// <summary>
    /// Configures Kestrel to listen on a dedicated health check port if enabled in configuration.
    /// Also handles preserving default application ports when Kestrel is manually configured.
    /// </summary>
    /// <param name="webHostBuilder">The web host builder.</param>
    /// <returns>The web host builder for chaining.</returns>
    public static IWebHostBuilder ConfigureHealthCheckPort(this IWebHostBuilder webHostBuilder)
    {
        return webHostBuilder.ConfigureKestrel((context, options) =>
        {
            var healthOptions = context.Configuration.GetSection(HealthOptions.Health).Get<HealthOptions>();

            // Only configure custom listening if health checks are enabled and a specific port is requested
            if (healthOptions?.Enabled == true && healthOptions.Port.HasValue)
            {
                // Bind to the configured health port
                options.ListenAnyIP(healthOptions.Port.Value);

                // IMPORTANT: When manually configuring Kestrel, we must explicitly re-bind 
                // the default application ports (e.g., from launchSettings or Docker), 
                // otherwise they are lost.

                // 1. Try ASPNETCORE_HTTP_PORTS (Docker default)
                var httpPorts = context.Configuration["ASPNETCORE_HTTP_PORTS"];
                if (!string.IsNullOrEmpty(httpPorts))
                {
                    foreach (var portStr in httpPorts.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(portStr, out int port))
                        {
                            options.ListenAnyIP(port);
                        }
                    }
                }

                // 2. Try ASPNETCORE_URLS or --urls (Development default)
                var urls = context.Configuration["ASPNETCORE_URLS"] ?? context.Configuration["urls"];
                if (!string.IsNullOrEmpty(urls))
                {
                    foreach (var url in urls.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        // Rudimentary parsing to extract port if it's an HTTP binding
                        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                        {
                            if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
                            {
                                options.Listen(IPAddress.Any, uri.Port);
                            }
                            // Note: HTTPS handling would require more logic for certificates
                        }
                    }
                }
            }
        });
    }

    /// <summary>
    /// Maps the Liveness and Readiness health check endpoints if enabled.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapIdentityHealthChecks(this WebApplication app)
    {
        var healthOptions = app.Configuration.GetSection(HealthOptions.Health).Get<HealthOptions>();

        if (healthOptions?.Enabled == true)
        {
            // Liveness Probe (Self)
            app.MapHealthChecks(healthOptions.LivePath, new HealthCheckOptions
            {
                // Exclude DB check for liveness
                Predicate = r => r.Name != "IdentityDbContext",
                ResponseWriter = WriteResponse
            });

            // Readiness Probe (Dependencies)
            app.MapHealthChecks(healthOptions.ReadyPath, new HealthCheckOptions
            {
                // Include DB check
                Predicate = r => r.Name == "IdentityDbContext",
                ResponseWriter = WriteResponse
            });
        }

        return app;
    }

    // Add this static readonly field to cache the JsonSerializerOptions instance
    private static readonly JsonSerializerOptions CachedJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private static Task WriteResponse(HttpContext context, HealthReport report)
    {
        IdentityApiOptions appOptions = context.RequestServices.GetService<IOptions<IdentityApiOptions>>()?.Value;

        context.Response.ContentType = "application/json";

        var response = new
        {
            Status = report.Status.ToString(),
            appOptions?.Version,
            appOptions?.ClientId,
            Duration = report.TotalDuration,
            Entries = report.Entries.Select(e => new
            {
                Key = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration,
                Data = e.Value.Data.Any() ? e.Value.Data : null
            })
        };

        // Use the cached JsonSerializerOptions instance
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, CachedJsonOptions));
    }
}
