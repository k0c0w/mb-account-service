using System.Data;
using Npgsql;
using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;
using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services.Domain;

public class AccountInterestRewarder(AccountServiceDbContext dbContext, IDomainEventNotifier eventNotifier)
    : IAccountInterestRewarder
{
    public async Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var accountIdParam = new NpgsqlParameter("p_account_id", NpgsqlTypes.NpgsqlDbType.Uuid)
            {
                Value = accountId,
                Direction = ParameterDirection.Input
            };

            var fromDateParam = new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };

            var toDateParam = new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };

            var amountAddedParam = new NpgsqlParameter("p_amount_added", NpgsqlTypes.NpgsqlDbType.Numeric)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };

            var wasAccruedParam = new NpgsqlParameter("p_was_accrued", NpgsqlTypes.NpgsqlDbType.Boolean)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };


            await dbContext.Database.ExecuteSqlRawAsync(
                "CALL accrue_interest(@p_account_id, @p_from_date, @p_to_date, @p_amount_added, @p_was_accrued);",
                accountIdParam, fromDateParam, toDateParam, amountAddedParam, wasAccruedParam
            );

            var wasAccrued = wasAccruedParam.Value as bool?;
            if (wasAccrued == true)
            {
                var from = fromDateParam.Value as DateTime? ?? throw new InvalidOperationException();
                var to = toDateParam.Value as DateTime? ?? throw new InvalidOperationException();
                var amount = amountAddedParam.Value as decimal? ?? throw new InvalidOperationException();

                var accrualEvent = EventsFactory.InterestAccruedV1(from, to, amount, accountId);
                await eventNotifier.NotifyAsync(accrualEvent);
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}