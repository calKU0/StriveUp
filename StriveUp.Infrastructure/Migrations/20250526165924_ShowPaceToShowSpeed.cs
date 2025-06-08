using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShowPaceToShowSpeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowPace",
                table: "UserActivities",
                newName: "ShowSpeed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowSpeed",
                table: "UserActivities",
                newName: "ShowPace");
        }
    }
}