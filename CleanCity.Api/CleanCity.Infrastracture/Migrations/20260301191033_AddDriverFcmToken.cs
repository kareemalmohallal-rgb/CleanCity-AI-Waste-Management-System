using CleanCity.Infrastructure.Data;
using CleanCity.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

#nullable disable
//Add - Migration AddDriverFcmToken - Project CleanCity.Infrastructure - StartupProject CleanCity.Api - Context applicationContext
//Update - Database AddDriverFcmToken - Project CleanCity.Infrastructure - StartupProject CleanCity.Api - Context applicationContext
namespace CleanCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverFcmToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FcmToken",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmToken",
                table: "Drivers");
        }
    }
}
