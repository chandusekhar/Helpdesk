using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddImportExportLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ImportExportLink",
                table: "SiteNavTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportExportLink",
                table: "SiteNavTemplates");
        }
    }
}
