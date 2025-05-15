using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivitySynchroField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SynchroId",
                table: "UserActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isManualAdded",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSynchronized",
                table: "UserActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SynchroId",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "isManualAdded",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "isSynchronized",
                table: "UserActivities");
        }
    }
}
