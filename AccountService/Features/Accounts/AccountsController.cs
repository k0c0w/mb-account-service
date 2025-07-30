using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[Route("accounts")]
public class AccountsController : ControllerBase
{
    [HttpPost("")]
    public async Task<IResult> CreateNewAsync(
        [FromBody] CreateNewAccountCommand request,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(request);
        return Results.Created("accounts", result);
    }

    [HttpGet("")]
    public async Task<IResult> ListAccountsAsync([FromServices] IMediator mediator)
    {
        var accounts = await mediator.Send(new GetAccountsQuery());

        return Results.Json(accounts);
    }

    [HttpPatch("{id:guid}/interest-rate")]
    public async Task<IResult> ChangeInterestRateAsync(
        [FromRoute] Guid id, 
        [FromBody] decimal value,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(new ChangeAccountInterestRateCommand(id, value));
        
        return Results.NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IResult> RemoveAccountAsync([FromRoute] Guid id, 
    [FromServices] IMediator mediator)
    {
        await mediator.Send(new RemoveAccountCommand(id));

        return Results.NoContent();
    }
}