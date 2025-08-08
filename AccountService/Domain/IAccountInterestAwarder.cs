namespace AccountService.Domain;

public interface IAccountInterestAwarder
{
    public Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default);
}