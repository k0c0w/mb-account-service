namespace AccountService.Features.Domain;

public interface IAccountInterestRewarder
{
    public Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default);
}