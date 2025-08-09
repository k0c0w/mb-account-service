namespace AccountService.Domain;

public interface IAccountInterestRewarder
{
    public Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default);
}