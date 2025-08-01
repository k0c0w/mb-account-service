using AccountService.Features;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AccountService.Swagger;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
internal sealed class AddDefaultResponsesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        Add500StatusCodeSchema(operation, context);

    }

    private static void Add500StatusCodeSchema(OpenApiOperation operation, OperationFilterContext context)
    {
        const string statusCode = "500";
        const string description = "Internal Server Error";

        if (operation.Responses.ContainsKey(statusCode))
        {
            return;
        }
        var errorSchema = context.SchemaGenerator.GenerateSchema(
            typeof(MbResult<string>), context.SchemaRepository);

        operation.Responses.Add(statusCode, new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
                {
                    Schema = errorSchema
                }
            }
        });
    }
}

