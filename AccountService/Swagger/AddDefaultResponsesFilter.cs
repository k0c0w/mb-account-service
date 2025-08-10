using System.Net;
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
        AddSchema(HttpStatusCode.InternalServerError, "Internal Server Error", operation, context);
        AddSchema(HttpStatusCode.Unauthorized, "Unauthorized Access", operation, context);
        AddSchema(HttpStatusCode.Conflict, "Concurrency Conflict", operation, context);
    }

    private static void AddSchema(HttpStatusCode statusCode, string description, OpenApiOperation operation, OperationFilterContext context)
    {
        var statusCodeString = ((int)statusCode).ToString();
        
        if (operation.Responses.ContainsKey(statusCodeString))
        {
            return;
        }
        var errorSchema = context.SchemaGenerator.GenerateSchema(
            typeof(MbResult<string>), context.SchemaRepository);

        operation.Responses.Add(statusCodeString, new OpenApiResponse
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

