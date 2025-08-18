using System.Text.Json;
using System.Text.RegularExpressions;
using AccountService.Features.AccountFeatures.SetClientAccountsFrozen;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Persistence.Infrastructure.RabbitMq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;

namespace AccountService.Consumers.Antifraud;

public class AntifraudConsumer(IServiceScopeFactory scopeFactory, ILogger<AntifraudConsumer> logger)
    : RabbitMqConsumer(scopeFactory, logger)
{
    protected override string QueueName => "account.antifraud";

    protected override async Task HandleMessageAsync(BasicDeliverEventArgs args)
    {
        var props = args.BasicProperties;
        var eventId = props.MessageId;
        var eventType = args.RoutingKey;
        var eventCorrelationId = props.CorrelationId;
        
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(args.CancellationToken);
        try
        {
            if (args.BasicProperties.ContentType == null 
                || !Regex.IsMatch(args.BasicProperties.ContentType, @"application/(\w+)?json"))
            {
                Logger.LogWarning("Unsupported content type {contentType}", args.BasicProperties.ContentType);
                throw new ArgumentException($"Unsupported content-type: {args.BasicProperties.ContentType}");
            }
            
            var stream = new MemoryStream(args.Body.ToArray());
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var e = await JsonSerializer.DeserializeAsync<Envelope<ClientStateChanged>>(stream, options);
            if (e == null || e.Meta.Version != "v1")
            {
                Logger.LogWarning("{EventId}:{Type}:{CorrelationId} received unsupported message version: {version}", 
                    e?.Id.ToString() ?? eventId, eventType, eventCorrelationId, e?.Meta.Version);
                
                var deadMessage = new InboxDeadMessage
                {
                    Error = "Unsupported message version",
                    Handler = nameof(AntifraudConsumer),
                    MessageId = e?.Id.ToString() ?? string.Empty,
                    ReceivedAt = DateTimeOffset.UtcNow,
                    Payload = JsonSerializer.Serialize(e)
                };
                await dbContext.InboxDeadMessages.AddAsync(deadMessage, args.CancellationToken);
                await dbContext.SaveChangesAsync(args.CancellationToken);
            }
            else
            {
                var messageWasNotProcessed = !await dbContext.InboxMessages
                    .AnyAsync(x => x.Id == e.Id, args.CancellationToken);
                
                if (messageWasNotProcessed)
                {
                    // todo: retries
                    await DispatchEvent(mediator, args.RoutingKey, e.Payload.ClientId, args.CancellationToken);
                    await dbContext.InboxMessages.AddAsync(new InboxMessage
                    {
                        Id = e.Id,
                        ProcessedAt = DateTimeOffset.UtcNow
                    }, args.CancellationToken);
                }
                else
                {
                    Logger.LogInformation("{EventId}:{Type}:{CorrelationId} skipping repeatable event", 
                        e?.Id.ToString() ?? eventId, eventType, eventCorrelationId);
                }
            }
            
            await transaction.CommitAsync(args.CancellationToken);
            Logger.LogInformation("{EventId}:{Type}:{CorrelationId} applied event", 
                e?.Id.ToString() ?? eventId, eventType, eventCorrelationId);

            await Channel.BasicAckAsync(args.DeliveryTag, multiple: false, args.CancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogInformation("{EventId}:{Type}:{CorrelationId} event handling failed with error: {Message}", 
                eventId, eventType, eventCorrelationId, e.Message);
            Logger.LogError(e, "Failure during message consuming: {message}", e.Message);
            await transaction.RollbackAsync(args.CancellationToken);

            await Channel.BasicRejectAsync(args.DeliveryTag, requeue: true);
        }
    }

    private static Task DispatchEvent(IMediator mediator, string routingKey, Guid clientId, CancellationToken ct)
    {
        return routingKey switch
        {
            "client.blocked" => mediator.Send(SetClientAccountsFrozenCommand.BlockAccounts(clientId), ct),
            "client.unblocked" => mediator.Send(SetClientAccountsFrozenCommand.UnblockAccounts(clientId), ct),
            _ => throw new ArgumentOutOfRangeException(nameof(routingKey),
                "Unknown routing key, can not dispatch events.", routingKey)
        };
    }
}