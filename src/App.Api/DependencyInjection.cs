using App.Api.Configuration;
using App.Api.Middleware;

namespace App.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddExceptionHandler<AppExceptionHandler>();
        services.AddExceptionHandler<UnhandledExceptionHandler>();
        services.AddProblemDetails();

        services.AddSwagger();

        return services;
    }
}
