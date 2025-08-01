using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.AccountStatements;

[Route("account-statements")]
public class AccountStatementsController(IMediator mediator)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetStatementForUserAsync([FromQuery] Guid accountId,
        [FromQuery] DateTimeOffset periodStartUtc,
        [FromQuery] DateTimeOffset? periodEndUtc
        )
    {
        periodEndUtc ??= DateTimeOffset.UtcNow;

        var statement = await mediator.Send(new GetAccountStatementQuery(accountId, periodStartUtc, periodEndUtc.Value));
        
        return Results.Ok(MbResult<AccountStatementDto, string[]>.Ok(statement));
    }
}