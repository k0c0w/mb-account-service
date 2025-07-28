using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features;

public sealed class RemoveAccount
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapDelete("/accounts/{id:guid}", async (HttpContext ctx, 
                [FromRoute] Guid id, 
                [FromServices] IMediator mediator) =>
            {
                await mediator.Send(new RemoveAccountCommand(id));
                ctx.Response.StatusCode = (int)HttpStatusCode.NoContent;
            })
            .WithDescription("Closes and deletes specific account.")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }
}