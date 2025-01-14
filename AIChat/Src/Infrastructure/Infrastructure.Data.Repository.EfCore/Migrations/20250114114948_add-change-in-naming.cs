using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Repository.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class addchangeinnaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "InvalidatedTokens",
                newName: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "InvalidatedTokens",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserTokens",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
