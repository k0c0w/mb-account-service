namespace AccountService.Features.Domain.Services;

public interface IAccountInterestRewarder
{
    public Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default);
}