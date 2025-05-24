using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Levels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Medals");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Medals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PointsPerMinute",
                table: "ActivityConfig",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            // 1. Create Levels table first
            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    XP = table.Column<int>(type: "int", nullable: false),
                    TotalXP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            // 2. Insert default level with Id = 1
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM Levels WHERE Id = 1)
        BEGIN
            SET IDENTITY_INSERT Levels ON;
            INSERT INTO Levels (Id, LevelNumber, XP, TotalXP)
            VALUES (1, 1, 0, 0);
            SET IDENTITY_INSERT Levels OFF;
        END
    ");

            // 3. Add columns after table exists
            migrationBuilder.AddColumn<int>(
                name: "CurrentXP",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // 4. Set LevelId for existing users (precautionary)
            migrationBuilder.Sql("UPDATE AspNetUsers SET LevelId = 1 WHERE LevelId IS NULL");

            // 5. Add FK constraint
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LevelId",
                table: "AspNetUsers",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Levels_LevelId",
                table: "AspNetUsers",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Levels_LevelId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LevelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Medals");

            migrationBuilder.DropColumn(
                name: "CurrentXP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PointsPerMinute",
                table: "ActivityConfig");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Medals",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}