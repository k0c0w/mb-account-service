using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features;

public class GetAccountsByOwnerId
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapGet("/users/{id:guid}/accounts", async (
                    [FromRoute] Guid id,
                    [FromServices] IMediator mediator) 
                => await mediator.Send(new GetAccountsQuery()))
            .WithDescription("Lists all active accounts of some user.")
            .Produces(200)
            .Produces(404)
            .Produces(500);
    }
}