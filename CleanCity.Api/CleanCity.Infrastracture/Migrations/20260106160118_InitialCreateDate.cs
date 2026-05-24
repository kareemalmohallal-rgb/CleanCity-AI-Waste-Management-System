using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymousDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceIdentifier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FcmToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CenterLatitude = table.Column<double>(type: "float", nullable: false),
                    CenterLongitude = table.Column<double>(type: "float", nullable: false),
                    RadiusInMeters = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AwarenessContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwarenessContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SewageServiceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SewageServiceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TruckTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    TruckId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CurrentDriverId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AnonymousDeviceId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AnonymousDevices_AnonymousDeviceId",
                        column: x => x.AnonymousDeviceId,
                        principalTable: "AnonymousDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reports_Drivers_CurrentDriverId",
                        column: x => x.CurrentDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TruckTypeId = table.Column<int>(type: "int", nullable: true),
                    DriverId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Trucks_TruckTypes_TruckTypeId",
                        column: x => x.TruckTypeId,
                        principalTable: "TruckTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AIAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    IsGarbageDetected = table.Column<bool>(type: "bit", nullable: false),
                    ConfidencePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIAnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIAnalysisResults_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportAssignments_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportAssignments_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedByType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportStatusHistories_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DeepLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    ReportAssignmentId = table.Column<int>(type: "int", nullable: true),
                    AwarenessContentId = table.Column<int>(type: "int", nullable: true),
                    AIAnalysisResultId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.CheckConstraint("CK_Notification_ExactlyOneSource", "(\r\n                        (CASE WHEN ReportId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN ReportAssignmentId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN AwarenessContentId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN AIAnalysisResultId IS NOT NULL THEN 1 ELSE 0 END)\r\n                      ) <= 1");
                    table.ForeignKey(
                        name: "FK_Notifications_AIAnalysisResults_AIAnalysisResultId",
                        column: x => x.AIAnalysisResultId,
                        principalTable: "AIAnalysisResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_AwarenessContents_AwarenessContentId",
                        column: x => x.AwarenessContentId,
                        principalTable: "AwarenessContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_ReportAssignments_ReportAssignmentId",
                        column: x => x.ReportAssignmentId,
                        principalTable: "ReportAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnonymousDeviceId = table.Column<int>(type: "int", nullable: true),
                    DriverId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceTokenId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => x.Id);
                    table.CheckConstraint("CK_NotificationRecipient_ExactlyOneTarget", "(\r\n                        (CASE WHEN AnonymousDeviceId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN DriverId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN DeviceTokenId IS NOT NULL THEN 1 ELSE 0 END) +\r\n                        (CASE WHEN UserId IS NOT NULL AND LTRIM(RTRIM(UserId)) <> '' THEN 1 ELSE 0 END)\r\n                      ) = 1");
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_AnonymousDevices_AnonymousDeviceId",
                        column: x => x.AnonymousDeviceId,
                        principalTable: "AnonymousDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_DeviceTokens_DeviceTokenId",
                        column: x => x.DeviceTokenId,
                        principalTable: "DeviceTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalysisResults_ReportId",
                table: "AIAnalysisResults",
                column: "ReportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousDevices_DeviceIdentifier",
                table: "AnonymousDevices",
                column: "DeviceIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId_IsActive",
                table: "DeviceTokens",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_AreaId",
                table: "Drivers",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ReportId",
                table: "Files",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_AnonymousDeviceId",
                table: "NotificationRecipients",
                column: "AnonymousDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_DeviceTokenId",
                table: "NotificationRecipients",
                column: "DeviceTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_DriverId",
                table: "NotificationRecipients",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId_IsRead",
                table: "NotificationRecipients",
                columns: new[] { "NotificationId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AIAnalysisResultId",
                table: "Notifications",
                column: "AIAnalysisResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AwarenessContentId",
                table: "Notifications",
                column: "AwarenessContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReportAssignmentId",
                table: "Notifications",
                column: "ReportAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReportId",
                table: "Notifications",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAssignments_DriverId_AssignedAt",
                table: "ReportAssignments",
                columns: new[] { "DriverId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportAssignments_ReportId_AssignedAt",
                table: "ReportAssignments",
                columns: new[] { "ReportId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AnonymousDeviceId",
                table: "Reports",
                column: "AnonymousDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AreaId",
                table: "Reports",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CurrentDriverId",
                table: "Reports",
                column: "CurrentDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportStatusHistories_ReportId_UpdatedAt",
                table: "ReportStatusHistories",
                columns: new[] { "ReportId", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_DriverId",
                table: "Trucks",
                column: "DriverId",
                unique: true,
                filter: "[DriverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Number",
                table: "Trucks",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckTypeId",
                table: "Trucks",
                column: "TruckTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "ReportStatusHistories");

            migrationBuilder.DropTable(
                name: "SewageServiceProviders");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "DeviceTokens");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "TruckTypes");

            migrationBuilder.DropTable(
                name: "AIAnalysisResults");

            migrationBuilder.DropTable(
                name: "AwarenessContents");

            migrationBuilder.DropTable(
                name: "ReportAssignments");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "AnonymousDevices");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
