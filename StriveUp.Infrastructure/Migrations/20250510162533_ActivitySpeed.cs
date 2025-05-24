using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivitySpeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "UserActivities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ActivitySpeeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserActivityId = table.Column<int>(type: "int", nullable: false),
                    SpeedValue = table.Column<double>(type: "float", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySpeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySpeeds_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySpeeds_UserActivityId",
                table: "ActivitySpeeds",
                column: "UserActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivitySpeeds");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "UserActivities");
        }
    }
}