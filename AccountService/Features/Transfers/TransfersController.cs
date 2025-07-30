using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transfers;

[Route("transfers")]
public class TransfersController : ControllerBase
{
    [HttpPost("")]
    public async Task<IResult> TransferMoneyAsync([FromBody] TransferMoneyCommand request, [FromServices] IMediator mediator)
    {
        await mediator.Send(request);

        return Results.Ok();
    }
}