using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using AccountService.Features.Domain.Events;

namespace AccountService.Swagger;

// ReSharper disable once ClassNeverInstantiated.Global
// Added to swagger UI
public class AddPossibleEventsDescriptionFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Components.Schemas.TryAdd(
            "AccountOpenedEvent",
            context.SchemaGenerator.GenerateSchema(
                typeof(EventEnvelope<AccountOpenedEvent>),
                context.SchemaRepository
            )
        );
        
        swaggerDoc.Components.Schemas.TryAdd(
            "InterestAccruedEvent",
            context.SchemaGenerator.GenerateSchema(
                typeof(EventEnvelope<InterestAccruedEvent>),
                context.SchemaRepository
            )
        );
        
        swaggerDoc.Components.Schemas.TryAdd(
            "MoneyCreditedEvent",
            context.SchemaGenerator.GenerateSchema(
                typeof(EventEnvelope<MoneyCreditedEvent>),
                context.SchemaRepository
            )
        );
        
        swaggerDoc.Components.Schemas.TryAdd(
            "MoneyDebitedEvent",
            context.SchemaGenerator.GenerateSchema(
                typeof(EventEnvelope<MoneyDebitedEvent>),
                context.SchemaRepository
            )
        );
        
        swaggerDoc.Components.Schemas.TryAdd(
            "TransferCompletedEvent",
            context.SchemaGenerator.GenerateSchema(
                typeof(EventEnvelope<TransferCompletedEvent>),
                context.SchemaRepository
            )
        );
    }
}
