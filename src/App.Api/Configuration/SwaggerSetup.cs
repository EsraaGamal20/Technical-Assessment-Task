using System.Reflection;
using App.Api.Configuration.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace App.Api.Configuration;

public static class SwaggerSetup
{
    private const string DocName = "v1";
    private const string Title   = "App API";
    private const string Version = "v1";

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocName, new OpenApiInfo
            {
                Title       = Title,
                Version     = Version,
                Description =
                    "Project & Task Management API.\n\n" +
                    "Authentication is two-step:\n" +
                    "1. `POST /api/auth/register/initiate` — creates an unverified user and sends an OTP.\n" +
                    "2. `POST /api/auth/register/verify-otp` — verifies the OTP and returns a JWT.\n" +
                    "Send the token as `Authorization: Bearer <token>` on protected endpoints."
            });

            var jwtScheme = new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                In           = ParameterLocation.Header,
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT",
                Description  = "Paste ONLY the JWT (Swagger UI adds the 'Bearer ' prefix automatically)."
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtScheme);

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, doc)] = new List<string>()
            });

            IncludeXmlIfExists(options, typeof(SwaggerSetup).Assembly);
            IncludeXmlIfExists(options, typeof(App.Application.DependencyInjection).Assembly);

            options.UseInlineDefinitionsForEnums();
            options.SchemaFilter<EnumSchemaFilter>();

            options.CustomSchemaIds(t => t.FullName!.Replace('+', '.'));
            options.OrderActionsBy(a => $"{a.GroupName}_{a.RelativePath}");

            options.OperationFilter<DefaultResponsesOperationFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerUiPipeline(
        this IApplicationBuilder app,
        IHostEnvironment env)
    {
        if (env.IsProduction()) return app;

        app.UseSwagger(opts =>
        {
            opts.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Title} {Version}");
            opts.DocumentTitle            = Title;
            opts.RoutePrefix              = "swagger";
            opts.DefaultModelsExpandDepth(-1);
            opts.DefaultModelExpandDepth(2);
            opts.DocExpansion(DocExpansion.None);
            opts.DisplayRequestDuration();
            opts.EnableFilter();
            opts.EnableDeepLinking();
        });

        return app;
    }

    private static void IncludeXmlIfExists(SwaggerGenOptions options, Assembly assembly)
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
}
