namespace AccountService.Persistence.Services.Infrastructure.Outbox;

public interface IOutboxProcessor
{
    Task ProcessMessagesAsync(CancellationToken ct = default);
}