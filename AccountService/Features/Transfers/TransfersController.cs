using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transfers;

[Route("transfers")]
public class TransfersController : ControllerBase
{
    /// <summary>
    /// Applies money transfer from account A to account B inside system.
    /// </summary>
    /// <response code="200">Transaction has been registered successfully</response>
    /// <response code="400">Invalid transaction or accounts` states</response>
    /// <response code="500">Some unhandled error</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> TransferMoneyAsync([FromBody] TransferMoneyCommand request, [FromServices] IMediator mediator)
    {
        await mediator.Send(request);

        return Results.Ok(MbResultWithError<string>.Ok());
    }
}