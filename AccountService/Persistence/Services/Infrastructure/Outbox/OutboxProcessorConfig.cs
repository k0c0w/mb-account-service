namespace AccountService.Persistence.Services.Infrastructure.Outbox;

public sealed record OutboxProcessorConfig
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // Configuring value
    public int BatchSize { get; init; } = 10;
}