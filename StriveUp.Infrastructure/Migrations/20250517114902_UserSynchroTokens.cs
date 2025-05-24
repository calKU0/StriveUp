using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserSynchroTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserSynchros");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserSynchros");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "UserSynchros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserSynchros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiresAt",
                table: "UserSynchros",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "UserSynchros");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserSynchros");

            migrationBuilder.DropColumn(
                name: "TokenExpiresAt",
                table: "UserSynchros");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserSynchros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserSynchros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}