using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddLicenses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicenseType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: true),
                    IsDeviceLicense = table.Column<bool>(type: "bit", nullable: false),
                    IsUserLicense = table.Column<bool>(type: "bit", nullable: false),
                    DeviceRequireProductCode = table.Column<bool>(type: "bit", nullable: false),
                    UserRequireProductCode = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLicenseAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpdeskUserId = table.Column<int>(type: "int", nullable: false),
                    LicenseTypeId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLicenseAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLicenseAssignment_HelpdeskUsers_HelpdeskUserId",
                        column: x => x.HelpdeskUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLicenseAssignment_LicenseType_LicenseTypeId",
                        column: x => x.LicenseTypeId,
                        principalTable: "LicenseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLicenseAssignment_HelpdeskUserId",
                table: "UserLicenseAssignment",
                column: "HelpdeskUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLicenseAssignment_LicenseTypeId",
                table: "UserLicenseAssignment",
                column: "LicenseTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLicenseAssignment");

            migrationBuilder.DropTable(
                name: "LicenseType");
        }
    }
}
