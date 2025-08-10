using AccountService.Features.Accounts.ChangeAccountInterestRateFeature;
using AccountService.Features.Accounts.CreateNewAccountFeature;
using AccountService.Features.Accounts.GetAccountsFeature;
using AccountService.Features.Accounts.RemoveAccountFeature;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[Route("accounts")]
public class AccountsController : ControllerBase
{
    /// <summary>
    /// Creates an account for existing user.
    /// </summary>
    /// <returns>A newly created account.</returns>
    /// <remarks>
    ///  Only existing user and supported currency will be accepted.
    /// </remarks>
    /// <response code="201">An account has been created</response>
    /// <response code="400">Invalid arguments or operation</response>
    /// <response code="500">Some unhandled error</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(MbResult<CreatedAccountDto, string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MbResult<CreatedAccountDto, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> CreateNewAsync(
        [FromBody] CreateNewAccountCommand request,
        [FromServices] IMediator mediator)
    {
        var createdAccount = await mediator.Send(request);
        
        var result = MbResult<CreatedAccountDto, string>.Ok(createdAccount);
        return Results.Created("accounts", result);
    }

    /// <summary>
    /// Returns all accounts for all users.
    /// </summary>
    /// <returns>List of existing accounts</returns>
    /// <response code="200">A list of existing accounts</response>
    /// <response code="500">Some unhandled error</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(MbResult<IEnumerable<AccountDto>, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> ListAccountsAsync([FromServices] IMediator mediator)
    {
        var accounts = await mediator.Send(new GetAccountsQuery());

        return Results.Json(MbResult<IEnumerable<AccountDto>, string>.Ok(accounts));
    }

    /// <summary>
    /// Updates rate of interest for an existing account. 
    /// </summary>
    /// <remarks>
    /// Checking accounts does not support operation.
    /// </remarks>
    /// <returns>Nothing</returns>
    /// <response code="204">Account has been updated successfully</response>
    /// <response code="400">Invalid argument met or account can not be updated</response>
    /// <response code="404">The account does not exist</response>
    /// <response code="409">The account has been modified already</response>
    /// <response code="500">Some unhandled error</response>
    [HttpPatch("{id:guid}/interest-rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult<string>),StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> ChangeInterestRateAsync(
        [FromRoute] Guid id, 
        [FromBody] decimal value,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(new ChangeAccountInterestRateCommand(id, value));
        
        return Results.NoContent();
    }
    
    /// <summary>
    /// Closes and deletes the account. 
    /// </summary>
    /// <returns>Nothing</returns>
    /// <response code="204">Account has been closed and deleted successfully</response>
    /// <response code="400">Invalid argument met or account can not be deleted</response>
    /// <response code="404">The account does not exist</response>
    /// <response code="409">The account has been modified already</response>
    /// <response code="500">Some unhandled error</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult<string>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult<string>),StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> RemoveAccountAsync([FromRoute] Guid id, 
    [FromServices] IMediator mediator)
    {
        await mediator.Send(new RemoveAccountCommand(id));

        return Results.NoContent();
    }
}