using System.Data;
using AccountService.Features.Domain;
using AccountService.Persistence.Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AccountService.Features.PipelineBehaviors;

public class TransactionalBehavior<TRequest,TResponse>(AccountServiceDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private DatabaseFacade Db => dbContext.Database;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {     
        const string transactionAbortedMessage = "Data inconsistency. Transaction was aborted.";

        if (request is not ITransactionalRequest || Db.CurrentTransaction is not null)
        {
            return await next(ct);
        }
        
        await using var transaction = await Db.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var response = await next(ct);
            await transaction.CommitAsync(ct);

            return response;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(ct);
            
            throw DomainException.CreateConcurrencyException(transactionAbortedMessage, ex);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}