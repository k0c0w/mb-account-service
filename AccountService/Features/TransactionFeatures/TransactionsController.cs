using AccountService.Features.TransactionFeatures.RegisterExternalTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.TransactionFeatures;

[Route("transactions")]
public class TransactionsController : ControllerBase
{
    /// <summary>
    /// Registers incoming/outgoing transaction related to the account.
    /// </summary>
    /// <remarks>
    /// This endpoint is used to register external transactions.
    /// Transactions are applied only to existing accounts.
    /// Transactions inside system should use another endpoint.
    /// </remarks>
    /// <response code="200">Transaction has been registered successfully</response>
    /// <response code="400">Invalid transaction or account state</response>
    /// <response code="401">unauthorized</response>
    /// <response code="409">The transaction was aborted due to data inconsistency</response>
    /// <response code="500">Some unhandled error</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> RegisterIncomingTransactionAsync(
        [FromBody] RegisterExternalTransactionCommand request,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(request);

        return Results.Ok(MbResultWithError<string>.Ok());
    }
}