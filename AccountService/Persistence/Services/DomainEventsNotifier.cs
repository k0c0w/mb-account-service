using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;
using MassTransit;

namespace AccountService.Persistence.Services;

public class DomainEventsNotifier(IPublishEndpoint publishEndpoint) : IDomainEventNotifier
{
    public Task NotifyAsync(IDomainEvent occuredEvent)
    {
        return occuredEvent switch
        {
            AccountOpenedEvent accountOpenedEvent => NotifyAsync(accountOpenedEvent),
            MoneyCreditedEvent moneyCreditedEvent => NotifyAsync(moneyCreditedEvent),
            MoneyDebitedEvent moneyDebitedEvent => NotifyAsync(moneyDebitedEvent),
            TransferCompletedEvent transferCompletedEvent => NotifyAsync(transferCompletedEvent),
            InterestAccruedEvent interestAccrued => NotifyAsync(interestAccrued),
            _ => throw new NotImplementedException("Unknown event met.")
        };
    }

    private Task NotifyAsync(AccountOpenedEvent accountOpenedEvent)
    {
        return publishEndpoint.Publish(accountOpenedEvent, ctx =>
        {
            ctx.SetRoutingKey("account.opened");
        });
    }
    
    private Task NotifyAsync(MoneyCreditedEvent moneyCredited)
    {
        return publishEndpoint.Publish(moneyCredited, ctx =>
        {
            ctx.SetRoutingKey("money.credited");
        });
    }
    
    private Task NotifyAsync(MoneyDebitedEvent moneyDebited)
    {
        return publishEndpoint.Publish(moneyDebited, ctx =>
        {
            ctx.SetRoutingKey("money.debited");
        });
    }
    
    private Task NotifyAsync(TransferCompletedEvent transferCompleted)
    {
        return publishEndpoint.Publish(transferCompleted, ctx =>
        {
            ctx.SetRoutingKey("money.transfer");
        });
    }
}