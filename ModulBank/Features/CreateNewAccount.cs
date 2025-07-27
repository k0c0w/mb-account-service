using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Features;

public sealed class CreateNewAccount
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapPost("/accounts", async (HttpContext ctx, 
                [FromBody] CreateNewAccountCommand request, 
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                ctx.Response.StatusCode = (int)HttpStatusCode.Created;
                return result;
            })
            .WithDescription("Creates new account for specific user.")
            .Produces(201)
            .Produces(400)
            .Produces(500);
    }
}