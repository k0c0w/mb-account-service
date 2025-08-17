using AccountService.Persistence.Services.Infrastructure.Outbox;
using Hangfire;

namespace AccountService.Jobs;

// ReSharper disable once ClassNeverInstantiated.Global
// Job is running by Hangfire
public class OutboxProcessingJob(IOutboxProcessor outboxProcessor)
{
    public const string Name = "account-service-outbox";
    
    public Task RunAsync(CancellationToken ct)
    {
        return outboxProcessor.ProcessMessagesAsync(ct);
    }
}