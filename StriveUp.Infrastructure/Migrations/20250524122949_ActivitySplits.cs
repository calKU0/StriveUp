using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivitySplits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivitySplits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserActivityId = table.Column<int>(type: "int", nullable: false),
                    SplitNumber = table.Column<int>(type: "int", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    AvgSpeed = table.Column<double>(type: "float", nullable: true),
                    AvgPace = table.Column<TimeSpan>(type: "time", nullable: true),
                    AvgHr = table.Column<int>(type: "int", nullable: true),
                    ElevationGain = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySplits_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySplits_UserActivityId",
                table: "ActivitySplits",
                column: "UserActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivitySplits");
        }
    }
}
