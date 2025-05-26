using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BestEffortToActivityLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserActivityId1",
                table: "BestEfforts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BestEfforts_UserActivityId1",
                table: "BestEfforts",
                column: "UserActivityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BestEfforts_UserActivities_UserActivityId1",
                table: "BestEfforts",
                column: "UserActivityId1",
                principalTable: "UserActivities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BestEfforts_UserActivities_UserActivityId1",
                table: "BestEfforts");

            migrationBuilder.DropIndex(
                name: "IX_BestEfforts_UserActivityId1",
                table: "BestEfforts");

            migrationBuilder.DropColumn(
                name: "UserActivityId1",
                table: "BestEfforts");
        }
    }
}
