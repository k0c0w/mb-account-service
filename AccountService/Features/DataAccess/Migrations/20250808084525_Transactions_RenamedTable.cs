using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Features.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Transactions_RenamedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Accounts_AccountId",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_TimeUtc",
                table: "Transactions",
                newName: "IX_Transactions_TimeUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_AccountId_TimeUtc",
                table: "Transactions",
                newName: "IX_Transactions_AccountId_TimeUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TimeUtc",
                table: "Transaction",
                newName: "IX_Transaction_TimeUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_AccountId_TimeUtc",
                table: "Transaction",
                newName: "IX_Transaction_AccountId_TimeUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Accounts_AccountId",
                table: "Transaction",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
