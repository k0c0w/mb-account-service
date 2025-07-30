using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[Route("transactions")]
public class TransactionsController : ControllerBase
{
    [HttpPost("")]
    public async Task<IResult> RegisterIncomingTransactionAsync(
        [FromBody] RegisterExternalTransactionCommand request,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(request);

        return Results.Ok();
    }
}