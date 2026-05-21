using App.Application.Features.Auth.Services;
using App.Application.Features.Projects.Services;
using App.Application.Features.Tasks.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            ServiceLifetime.Scoped,
            includeInternalTypes: false);

        services.AddScoped<IAuthService,    AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService,    TaskService>();

        return services;
    }
}
