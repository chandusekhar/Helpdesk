using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class FixUserLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLicenseAssignment_HelpdeskUsers_HelpdeskUserId",
                table: "UserLicenseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLicenseAssignment_LicenseType_LicenseTypeId",
                table: "UserLicenseAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLicenseAssignment",
                table: "UserLicenseAssignment");

            migrationBuilder.RenameTable(
                name: "UserLicenseAssignment",
                newName: "UserLicenseAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_UserLicenseAssignment_LicenseTypeId",
                table: "UserLicenseAssignments",
                newName: "IX_UserLicenseAssignments_LicenseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLicenseAssignment_HelpdeskUserId",
                table: "UserLicenseAssignments",
                newName: "IX_UserLicenseAssignments_HelpdeskUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLicenseAssignments",
                table: "UserLicenseAssignments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLicenseAssignments_HelpdeskUsers_HelpdeskUserId",
                table: "UserLicenseAssignments",
                column: "HelpdeskUserId",
                principalTable: "HelpdeskUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLicenseAssignments_LicenseType_LicenseTypeId",
                table: "UserLicenseAssignments",
                column: "LicenseTypeId",
                principalTable: "LicenseType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLicenseAssignments_HelpdeskUsers_HelpdeskUserId",
                table: "UserLicenseAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLicenseAssignments_LicenseType_LicenseTypeId",
                table: "UserLicenseAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLicenseAssignments",
                table: "UserLicenseAssignments");

            migrationBuilder.RenameTable(
                name: "UserLicenseAssignments",
                newName: "UserLicenseAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_UserLicenseAssignments_LicenseTypeId",
                table: "UserLicenseAssignment",
                newName: "IX_UserLicenseAssignment_LicenseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLicenseAssignments_HelpdeskUserId",
                table: "UserLicenseAssignment",
                newName: "IX_UserLicenseAssignment_HelpdeskUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLicenseAssignment",
                table: "UserLicenseAssignment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLicenseAssignment_HelpdeskUsers_HelpdeskUserId",
                table: "UserLicenseAssignment",
                column: "HelpdeskUserId",
                principalTable: "HelpdeskUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLicenseAssignment_LicenseType_LicenseTypeId",
                table: "UserLicenseAssignment",
                column: "LicenseTypeId",
                principalTable: "LicenseType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
