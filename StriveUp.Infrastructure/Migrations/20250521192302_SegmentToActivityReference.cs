using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SegmentToActivityReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "SegmentConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SegmentConfigs_ActivityId",
                table: "SegmentConfigs",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SegmentConfigs_Activities_ActivityId",
                table: "SegmentConfigs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SegmentConfigs_Activities_ActivityId",
                table: "SegmentConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SegmentConfigs_ActivityId",
                table: "SegmentConfigs");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "SegmentConfigs");
        }
    }
}
