using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriveUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SegmentDistanceColummnTypeChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DistanceMeters",
                table: "SegmentConfigs",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DistanceMeters",
                table: "SegmentConfigs",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}