using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedalsEarned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedalEarned_Activities_ActivityId",
                table: "MedalEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalEarned_AspNetUsers_AppUserId",
                table: "MedalEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalEarned_AspNetUsers_UserId",
                table: "MedalEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalEarned_Medals_MedalId",
                table: "MedalEarned");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedalEarned",
                table: "MedalEarned");

            migrationBuilder.RenameTable(
                name: "MedalEarned",
                newName: "MedalsEarned");

            migrationBuilder.RenameIndex(
                name: "IX_MedalEarned_UserId",
                table: "MedalsEarned",
                newName: "IX_MedalsEarned_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalEarned_MedalId",
                table: "MedalsEarned",
                newName: "IX_MedalsEarned_MedalId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalEarned_AppUserId",
                table: "MedalsEarned",
                newName: "IX_MedalsEarned_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalEarned_ActivityId",
                table: "MedalsEarned",
                newName: "IX_MedalsEarned_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedalsEarned",
                table: "MedalsEarned",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalsEarned_Activities_ActivityId",
                table: "MedalsEarned",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_MedalsEarned_Medals_MedalId",
                table: "MedalsEarned",
                column: "MedalId",
                principalTable: "Medals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_Activities_ActivityId",
                table: "MedalsEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_AppUserId",
                table: "MedalsEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_AspNetUsers_UserId",
                table: "MedalsEarned");

            migrationBuilder.DropForeignKey(
                name: "FK_MedalsEarned_Medals_MedalId",
                table: "MedalsEarned");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedalsEarned",
                table: "MedalsEarned");

            migrationBuilder.RenameTable(
                name: "MedalsEarned",
                newName: "MedalEarned");

            migrationBuilder.RenameIndex(
                name: "IX_MedalsEarned_UserId",
                table: "MedalEarned",
                newName: "IX_MedalEarned_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalsEarned_MedalId",
                table: "MedalEarned",
                newName: "IX_MedalEarned_MedalId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalsEarned_AppUserId",
                table: "MedalEarned",
                newName: "IX_MedalEarned_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedalsEarned_ActivityId",
                table: "MedalEarned",
                newName: "IX_MedalEarned_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedalEarned",
                table: "MedalEarned",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalEarned_Activities_ActivityId",
                table: "MedalEarned",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedalEarned_AspNetUsers_AppUserId",
                table: "MedalEarned",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedalEarned_AspNetUsers_UserId",
                table: "MedalEarned",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedalEarned_Medals_MedalId",
                table: "MedalEarned",
                column: "MedalId",
                principalTable: "Medals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
