using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Repository.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class addchangeindb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_AspNetUsers_UserId",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserTokens",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokens",
                newName: "IX_UserTokens_ApplicationUserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_AspNetUsers_ApplicationUserId",
                table: "UserTokens",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_AspNetUsers_ApplicationUserId",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "UserTokens",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokens_ApplicationUserId",
                table: "UserTokens",
                newName: "IX_UserTokens_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_AspNetUsers_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
