using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class ChangeFileUploadDetectedTypeToMIMETYpe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DetectedFileType",
                table: "FileUploads",
                newName: "MIMEType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MIMEType",
                table: "FileUploads",
                newName: "DetectedFileType");
        }
    }
}
