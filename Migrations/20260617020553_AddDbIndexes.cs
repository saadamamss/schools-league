using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolsLeague.Migrations
{
    /// <inheritdoc />
    public partial class AddDbIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_UserId_TransactionDate",
                table: "FinancialTransactions",
                columns: new[] { "UserId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_Date_Status",
                table: "Attendances",
                columns: new[] { "Date", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_LocationId_Date",
                table: "Attendances",
                columns: new[] { "LocationId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FinancialTransactions_UserId_TransactionDate",
                table: "FinancialTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_Date_Status",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_LocationId_Date",
                table: "Attendances");
        }
    }
}
