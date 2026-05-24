using CleanCity.Infrastructure.Data;
using CleanCity.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

#nullable disable
//Add - Migration UpdateInitialCreateDate - Project CleanCity.Infrastructure - StartupProject CleanCity.Api - Context applicationContext
//Update - Database UpdateInitialCreateDate - Project CleanCity.Infrastructure - StartupProject CleanCity.Api - Context applicationContext
namespace CleanCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialCreateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "RadiusInMeters",
                table: "Areas",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "CenterLongitude",
                table: "Areas",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "CenterLatitude",
                table: "Areas",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "MaxLat",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MaxLng",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinLat",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinLng",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxLat",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "MaxLng",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "MinLat",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "MinLng",
                table: "Areas");

            migrationBuilder.AlterColumn<double>(
                name: "RadiusInMeters",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CenterLongitude",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CenterLatitude",
                table: "Areas",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
