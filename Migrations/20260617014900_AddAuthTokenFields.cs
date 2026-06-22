using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolsLeague.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Users",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifyToken",
                table: "Users",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifyTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResetToken",
                table: "Users",
                column: "ResetToken",
                unique: true,
                filter: "[ResetToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_VerifyToken",
                table: "Users",
                column: "VerifyToken",
                unique: true,
                filter: "[VerifyToken] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ResetToken",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_VerifyToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerifyToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerifyTokenExpiry",
                table: "Users");
        }
    }
}
