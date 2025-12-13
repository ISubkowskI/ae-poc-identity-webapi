using Ae.Poc.Identity.DbContexts;
using Ae.Poc.Identity.Extensions;
using Ae.Poc.Identity.Settings;
using Serilog;

var logConfig = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(standardErrorFromLevel: Serilog.Events.LogEventLevel.Verbose);
Log.Logger = logConfig.CreateBootstrapLogger();

try
{
    Log.Information("App starting ... '{Env}'", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty);

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services
        .AddAppConfiguration(builder.Configuration)
        .AddSqliteDbContext<IIdentityDbContext, IdentityDbContext>()
        .AddAppMapper()
        .AddAppServices()
        .AddIdentityHealthChecks(); // Replaces inline AddHealthChecks

    var appOptions = builder.Configuration.GetSection(IdentityApiOptions.App).Get<IdentityApiOptions>() ?? new IdentityApiOptions();
    Log.Information("{AppTitle} ver:'{AppVersion}'", appOptions.Title, appOptions.Version);

    // Configure Kestrel to listen on the health port if enabled
    // This logic is now encapsulated in the extension method
    builder.WebHost.ConfigureHealthCheckPort();

    builder.Services.AddControllersWithViews();
    builder.Services.AddAntiforgery();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Add CORS policy
    var AllowAnySpecificOriginsPolicy = "_allowAnySpecificOriginsPolicy";
    builder.Services.AddCors(o => o.AddPolicy(name: AllowAnySpecificOriginsPolicy, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseCors(AllowAnySpecificOriginsPolicy);
    app.UseAuthorization();

    app.MapControllers();

    // Map Health Checks
    app.MapIdentityHealthChecks();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        // creates schema
        db.Database.EnsureCreated();
        // inserts sample data
        db.Seed();
    }

    await app.RunAsync();
    return 0;
}
catch (Exception exc)
{
    Log.Fatal(exc, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
