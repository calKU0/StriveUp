using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ElevationAndBestEfforts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityElevations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserActivityId = table.Column<int>(type: "int", nullable: false),
                    ElevationValue = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityElevations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityElevations_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SegmentConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegmentConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BestEfforts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserActivityId = table.Column<int>(type: "int", nullable: false),
                    SegmentConfigId = table.Column<int>(type: "int", nullable: false),
                    DurationSeconds = table.Column<double>(type: "float", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestEfforts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BestEfforts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BestEfforts_SegmentConfigs_SegmentConfigId",
                        column: x => x.SegmentConfigId,
                        principalTable: "SegmentConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BestEfforts_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityElevations_UserActivityId",
                table: "ActivityElevations",
                column: "UserActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_BestEfforts_SegmentConfigId",
                table: "BestEfforts",
                column: "SegmentConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BestEfforts_UserActivityId",
                table: "BestEfforts",
                column: "UserActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_BestEfforts_UserId",
                table: "BestEfforts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityElevations");

            migrationBuilder.DropTable(
                name: "BestEfforts");

            migrationBuilder.DropTable(
                name: "SegmentConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}