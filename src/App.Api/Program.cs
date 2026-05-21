using App.Api;
using App.Api.Configuration;
using App.Api.Filters;
using App.Application;
using App.Infrastructure;
using App.Persistence;
using App.Persistence.Context;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Configuration 
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                 optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

//  Services 
builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices();

builder.Services.AddAppCors(builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Auto-migrate + seed in Development 
if (app.Environment.IsDevelopment())
{
    try
    {
        await using var scope  = app.Services.CreateAsyncScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher>();
        var logger = sp.GetRequiredService<ILogger<Program>>();
        await db.Database.MigrateAsync();
        await App.Persistence.Seeding.DatabaseSeeder.SeedAsync(db, hasher, logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Auto-migration/seed failed — database may be unavailable.");
    }
}

//  Middleware pipeline 
app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(CorsSetup.DefaultPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwaggerUiPipeline(app.Environment);

app.MapGet("/", () => Results.Ok(new
{
    name        = "App API",
    version     = "v1",
    environment = app.Environment.EnvironmentName,
    timeUtc     = DateTime.UtcNow
}))
.WithName("Root")
.AllowAnonymous()
.ExcludeFromDescription();

app.Logger.LogInformation(
    "App API started. Env={Env}  Swagger=/swagger",
    app.Environment.EnvironmentName);

await app.RunAsync();
