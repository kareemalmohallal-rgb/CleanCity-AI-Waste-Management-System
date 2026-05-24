using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialCreateDate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterLatitude",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "CenterLongitude",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "RadiusInMeters",
                table: "Areas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CenterLatitude",
                table: "Areas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CenterLongitude",
                table: "Areas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RadiusInMeters",
                table: "Areas",
                type: "float",
                nullable: true);
        }
    }
}
