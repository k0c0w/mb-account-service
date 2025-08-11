using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Features.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_AccrueInterest_Procedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                     CREATE OR REPLACE PROCEDURE accrue_interest(p_account_id UUID)
                     LANGUAGE plpgsql
                     AS $$
                     DECLARE
                         acc RECORD;
                         last_accrual_date DATE;
                         days_missing INTEGER ;
                         new_amount NUMERIC;
                         total_interest NUMERIC;
                        time timestamp;
                        transaction_type smallint;
                     BEGIN
                        -- Find an account
                         SELECT * INTO acc
                           FROM "Accounts"
                          WHERE "Id" = p_account_id
                                AND "InterestRate" IS NOT NULL 
                                AND "InterestRate" != 0
                         FOR UPDATE;
                     
                         IF NOT FOUND THEN
                             RETURN;
                         END IF;
                     
                         -- Get last interest accrual date
                         SELECT DATE("TimeUtc") INTO last_accrual_date
                           FROM "Transactions"
                          WHERE "AccountId" = acc."Id"
                                AND "Description" ILIKE 'interest accrual%'
                          ORDER BY "TimeUtc" DESC
                          LIMIT 1;
                     
                         -- Use account creation date if no accrual exists
                         IF last_accrual_date IS NULL THEN
                             last_accrual_date := DATE(acc."CreationTimeUtc");
                         END IF;
                     
                         -- Number of full days since last accrual
                         days_missing := CURRENT_DATE - last_accrual_date;
                     
                        -- if accrual needed
                         IF days_missing > 0 THEN
                             new_amount := acc."Amount" * power(1 + (acc."InterestRate" / 365)::NUMERIC, days_missing);
                             total_interest := new_amount - acc."Amount";
                     
                            IF total_interest = 0 THEN
                                RETURN;
                            END IF;
                     		
                            time := now() AT TIME ZONE 'UTC';
                             -- Update account
                             UPDATE "Accounts"
                                SET   "Amount" = new_amount
                                    , "ModifiedAt" = time
                              WHERE "Id" = acc."Id";
                     
                            IF total_interest > 0 THEN
                                transaction_type := 1 /* Debit */;
                            ELSE
                                transaction_type := 2 /* Credit */;
                                total_interest := ABS(total_interest);
                            END IF;
                     			
                             -- Record transaction
                             INSERT INTO "Transactions"(
                             "Id", "AccountId", "Description", "Amount", "CurrencyCode", "TimeUtc", "Type"
                             ) VALUES (
                                 gen_random_uuid(),
                                 acc."Id",
                                FORMAT('Interest accrual for %s day(s).', days_missing),
                                 total_interest,
                                 acc."CurrencyCode",
                                 time,
                                 transaction_type
                             );
                         END IF;
                     END;
                     $$;
                     """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS AccrueInterest;");
        }
    }
}
