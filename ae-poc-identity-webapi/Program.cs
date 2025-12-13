using Ae.Poc.Identity.DbContexts;
using Ae.Poc.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAppConfiguration(builder.Configuration)
    .AddSqliteDbContext<IIdentityDbContext, IdentityDbContext>()
    .AddAppMapper()
    .AddAppServices();

builder.Services.AddControllers();
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    // creates schema
    db.Database.EnsureCreated();
    // inserts sample data
    db.Seed();
}

await app.RunAsync();
