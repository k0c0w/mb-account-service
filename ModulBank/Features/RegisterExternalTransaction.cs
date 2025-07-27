using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Features;

public class RegisterExternalTransaction
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapPost("/transactions", async (
                [FromBody] RegisterExternalTransactionCommand request,
                [FromServices] IMediator mediator) =>
                await mediator.Send(request)
            )
            .WithDescription("Registers external incoming transaction.")
            .Produces(200)
            .Produces(400)
            .Produces(500);
    }
}