using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvarageHr",
                table: "UserActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AvarageSpeed",
                table: "UserActivities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxHr",
                table: "UserActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxSpeed",
                table: "UserActivities",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvarageHr",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "AvarageSpeed",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "MaxHr",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "MaxSpeed",
                table: "UserActivities");
        }
    }
}