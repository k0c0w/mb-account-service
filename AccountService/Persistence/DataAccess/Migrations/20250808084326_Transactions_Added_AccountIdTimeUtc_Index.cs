using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Persistence.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Transactions_Added_AccountIdTimeUtc_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_AccountId",
                table: "Transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountId_TimeUtc",
                table: "Transaction",
                columns: new[] { "AccountId", "TimeUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_AccountId_TimeUtc",
                table: "Transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountId",
                table: "Transaction",
                column: "AccountId");
        }
    }
}
