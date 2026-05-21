using App.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Api.Configuration;

public static class CorsSetup
{
    public const string DefaultPolicy = "DefaultCorsPolicy";

    public static IServiceCollection AddAppCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var origins = configuration
            .GetSection(CorsSettings.SectionName)
            .Get<CorsSettings>()?.AllowedOrigins
            ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultPolicy, policy =>
            {
                if (origins.Length == 0)
                {
                    policy.SetIsOriginAllowed(o =>
                        Uri.TryCreate(o, UriKind.Absolute, out var uri)
                        && (uri.Host == "localhost" || uri.Host == "127.0.0.1"));
                }
                else
                {
                    policy.WithOrigins(origins);
                }

                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
