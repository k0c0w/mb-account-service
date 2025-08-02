using AccountService.Features.Users.GetAccountsByOwnerIdFeature;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Users;

[Microsoft.AspNetCore.Components.Route("users")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Allows to check some user`s opened accounts.
    /// </summary>
    /// <returns>All accounts opened for specific user</returns>
    /// <response code="200">List of all accounts opened for specific user</response>
    /// <response code="400">Invalid id has been provided</response>
    /// <response code="404">User is not found in system</response>
    /// <response code="500">Some unhandled error</response>
    [HttpGet("{id:guid}/accounts")]
    [ProducesResponseType(typeof(MbResult<IEnumerable<AccountByOwnerIdDto>, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetUserAccountsAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var accounts = await mediator.Send(new GetAccountsByOwnerIdQuery(id));
    
        return Results.Json(MbResult<IEnumerable<AccountByOwnerIdDto>, string>.Ok(accounts));
    }
}