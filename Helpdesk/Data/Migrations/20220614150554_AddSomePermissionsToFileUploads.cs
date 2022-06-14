using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddSomePermissionsToFileUploads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAllAuthenticatedAccess",
                table: "FileUploads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowUnauthenticatedAccess",
                table: "FileUploads",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAllAuthenticatedAccess",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "AllowUnauthenticatedAccess",
                table: "FileUploads");
        }
    }
}
