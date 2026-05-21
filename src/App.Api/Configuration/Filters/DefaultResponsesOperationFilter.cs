using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Api.Configuration.Filters;

public sealed class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("500", new OpenApiResponse
        {
            Description = "Unexpected server error."
        });

        var hasAuth = context.MethodInfo.DeclaringType is not null
            && (context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                || context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

        var hasAllowAnon = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (hasAuth && !hasAllowAnon)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse
            {
                Description = "Authentication is required or the token is invalid."
            });
        }
    }
}
