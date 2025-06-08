using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConfigs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Theme = table.Column<int>(type: "int", nullable: false),
                    DefaultMetric = table.Column<int>(type: "int", nullable: false),
                    PrivateMap = table.Column<bool>(type: "bit", nullable: false),
                    PrivateActivities = table.Column<bool>(type: "bit", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: true),
                    SensorName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfigs", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserConfigs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConfigs");
        }
    }
}