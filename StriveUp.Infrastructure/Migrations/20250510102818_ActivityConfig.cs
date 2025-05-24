using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "UserActivities");

            migrationBuilder.AddColumn<double>(
                name: "DurationSeconds",
                table: "UserActivities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "ActivityConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeasurementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultDistanceUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseHeartRate = table.Column<bool>(type: "bit", nullable: false),
                    ElevationRelevant = table.Column<bool>(type: "bit", nullable: false),
                    IndoorCapable = table.Column<bool>(type: "bit", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityConfig_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityHrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserActivityId = table.Column<int>(type: "int", nullable: false),
                    HearthRateValue = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityHrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityHrs_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityConfig_ActivityId",
                table: "ActivityConfig",
                column: "ActivityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityHrs_UserActivityId",
                table: "ActivityHrs",
                column: "UserActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityConfig");

            migrationBuilder.DropTable(
                name: "ActivityHrs");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "UserActivities");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "UserActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}