using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Features;

public class TransferMoney
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapPost("/transfers", async ( 
                [FromBody] TransferMoneyCommand request,
                [FromServices] IMediator mediator) => await mediator.Send(request)
            )
            .WithDescription("Transfers money between accounts.")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }
}