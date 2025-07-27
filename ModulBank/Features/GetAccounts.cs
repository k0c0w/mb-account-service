using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Features;

public sealed class GetAccounts
{
    public static void RegisterHttpEndpoint(IEndpointRouteBuilder b)
    {
        b.MapGet("/accounts", async ([FromServices] IMediator mediator) 
                => await mediator.Send(new GetAccountsQuery()))
            .WithDescription("Lists all active accounts.")
            .Produces(200)
            .Produces(500);
    }
}