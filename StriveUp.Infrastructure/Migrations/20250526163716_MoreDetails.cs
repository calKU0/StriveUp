using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoreDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SensorName",
                table: "UserActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowCalories",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowHeartRate",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPace",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SynchroProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "SensorName",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "ShowCalories",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "ShowHeartRate",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "ShowPace",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SynchroProviders");
        }
    }
}