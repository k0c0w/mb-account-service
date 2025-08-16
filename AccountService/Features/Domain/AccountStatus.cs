namespace AccountService.Features.Domain;

public enum AccountStatus : ushort
{
    Active = 1,
    Frozen = 2,
    Closed = 3
}