using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeActivityConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndoorCapable",
                table: "ActivityConfig",
                newName: "SpeedRelevant");

            migrationBuilder.AddColumn<int>(
                name: "ElevationGain",
                table: "UserActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DistanceRelevant",
                table: "ActivityConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Indoor",
                table: "ActivityConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElevationGain",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "DistanceRelevant",
                table: "ActivityConfig");

            migrationBuilder.DropColumn(
                name: "Indoor",
                table: "ActivityConfig");

            migrationBuilder.RenameColumn(
                name: "SpeedRelevant",
                table: "ActivityConfig",
                newName: "IndoorCapable");
        }
    }
}
