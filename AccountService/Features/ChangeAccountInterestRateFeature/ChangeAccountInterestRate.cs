using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features;

public class ChangeAccountInterestRate
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapPatch("/accounts/{id:guid}/interest-rate", async (HttpContext ctx, 
                [FromRoute] Guid id, 
                [FromBody] decimal value,
                [FromServices] IMediator mediator) =>
            {
                await mediator.Send(new ChangeAccountInterestRateCommand(id, value));
                ctx.Response.StatusCode = (int)HttpStatusCode.NoContent;
            })
            .WithDescription("Closes and deletes specific account.")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }
}