using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Users;

[Microsoft.AspNetCore.Components.Route("users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id:guid}/accounts")]
    public async Task<IResult> GetUserAccountsAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var accounts = await mediator.Send(new GetAccountsByOwnerIdQuery(id));
    
        return Results.Json(MbResult<IEnumerable<AccountByOwnerIdDto>, string[]>.Ok(accounts));
    }
}