using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserForeignKey1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_AppUserId",
                table: "MedalsEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_UserId",
                table: "MedalsEarned");

            migrationBuilder.DropIndex(
                name: "IX_MedalsEarned_AppUserId",
                table: "MedalsEarned");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "MedalsEarned");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_UserId",
                table: "MedalsEarned",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_UserId",
                table: "MedalsEarned");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "MedalsEarned",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedalsEarned_AppUserId",
                table: "MedalsEarned",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_AppUserId",
                table: "MedalsEarned",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_UserId",
                table: "MedalsEarned",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}