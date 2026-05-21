using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Api.Configuration.Filters;

public sealed class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        if (schema is not OpenApiSchema openApiSchema) return;

        openApiSchema.Enum = new List<JsonNode>();
        foreach (var name in Enum.GetNames(context.Type))
            openApiSchema.Enum.Add(JsonValue.Create(name)!);

        openApiSchema.Type   = JsonSchemaType.String;
        openApiSchema.Format = null;

        var members = Enum.GetValues(context.Type)
            .Cast<object>()
            .Select(v => $"{Convert.ToInt32(v)} = {v}");

        openApiSchema.Description = (openApiSchema.Description is null ? string.Empty : openApiSchema.Description + "\n\n")
            + "Allowed values: " + string.Join(", ", members);
    }
}
