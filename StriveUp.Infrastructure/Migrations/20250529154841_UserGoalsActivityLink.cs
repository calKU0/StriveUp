using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserGoalsActivityLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "UserGoals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_ActivityId",
                table: "UserGoals",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoals_Activities_ActivityId",
                table: "UserGoals",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGoals_Activities_ActivityId",
                table: "UserGoals");

            migrationBuilder.DropIndex(
                name: "IX_UserGoals_ActivityId",
                table: "UserGoals");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "UserGoals");
        }
    }
}
