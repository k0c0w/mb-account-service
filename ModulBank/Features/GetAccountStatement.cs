using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Features;

public sealed class GetAccountStatement
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapGet("/account-statements", async (
                    [FromQuery] Guid accountId,
                    [FromQuery] DateTimeOffset periodStartUtc,
                    [FromQuery] DateTimeOffset? periodEndUtc,
                    [FromServices] IMediator mediator) 
                =>
            {
                periodEndUtc ??= DateTimeOffset.UtcNow;

                return await mediator.Send(new GetAccountStatementQuery(accountId, periodStartUtc, periodEndUtc.Value));
            })
            .WithDescription("Calculates statement for some account.")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }
}