namespace App.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
        services.Configure<OtpSettings>(configuration.GetSection(OtpSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redis = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            if (string.IsNullOrWhiteSpace(redis.ConnectionString))
                throw new InvalidOperationException("RedisSettings:ConnectionString is missing.");

            var config = ConfigurationOptions.Parse(redis.ConnectionString, ignoreUnknown: true);
            config.AbortOnConnectFail = false;
            config.ConnectRetry       = 3;
            config.ConnectTimeout     = 5000;

            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IOtpService, OtpService>();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddJwtAuthentication(configuration);

        return services;
    }
}
