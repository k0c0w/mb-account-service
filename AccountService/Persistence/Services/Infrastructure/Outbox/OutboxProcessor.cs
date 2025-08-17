using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace AccountService.Persistence.Services.Infrastructure.Outbox;

public abstract class OutboxProcessor(AccountServiceDbContext dbContext,
    ILogger<IOutboxProcessor> logger,
    IOptions<OutboxProcessorConfig> config) 
    : IOutboxProcessor
{
    private DbSet<OutboxMessage> OutboxMessages => dbContext.OutboxMessages;
    
    private DatabaseFacade Database => dbContext.Database;

    private DbContext UnitOfWork => dbContext;

    private ILogger Logger => logger;
    
    public async Task ProcessMessagesAsync(CancellationToken ct = default)
    {
        await using var transaction = await Database.BeginTransactionAsync(ct);
        try
        {
            var messages = await OutboxMessages
                .Where(o => o.ProcessedAtUtc == null)
                .OrderBy(o => o.OccuredAtUtc)
                .Take(config.Value.BatchSize)
                .ToArrayAsync(ct);

            foreach (var outboxMessage in messages)
            {
                await PublishMessageAsync(outboxMessage, ct);
                outboxMessage.ProcessedAtUtc = DateTimeOffset.UtcNow;

                OutboxMessages.Update(outboxMessage);
            }

            await UnitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occured in outbox processor: {message}", ex.Message);
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    protected abstract ValueTask PublishMessageAsync(OutboxMessage outboxMessage, CancellationToken ct = default);
}