using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.AccountStatements;

[Route("account-statements")]
public class AccountStatementsController(IMediator mediator)
    : ControllerBase
{
    /// <summary>
    /// Calculates a statement for the account for selected period.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="periodStartUtc"></param>
    /// <param name="periodEndUtc"></param>
    /// <returns>An account statement for selected period.</returns>
    /// <response code="200">An account statement</response>
    /// <response code="400">Invalid statement bounds</response>
    /// <response code="404">An account was not found</response>
    /// <response code="500">Some unhandled error</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(MbResult<AccountStatementDto, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetStatementForUserAsync([FromQuery] Guid accountId,
        [FromQuery] DateTimeOffset periodStartUtc,
        [FromQuery] DateTimeOffset? periodEndUtc
        )
    {
        periodEndUtc ??= DateTimeOffset.UtcNow;

        var statement = await mediator.Send(new GetAccountStatementQuery(accountId, periodStartUtc, periodEndUtc.Value));
        
        return Results.Ok(MbResult<AccountStatementDto, string>.Ok(statement));
    }
}