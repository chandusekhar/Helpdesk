using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class ChangeSettingNavMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteOptionsLink",
                table: "SiteNavTemplates",
                newName: "SiteSettingsLink");

            migrationBuilder.AddColumn<bool>(
                name: "LicenseTypeLink",
                table: "SiteNavTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowConfigurationMenu",
                table: "SiteNavTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseTypeLink",
                table: "SiteNavTemplates");

            migrationBuilder.DropColumn(
                name: "ShowConfigurationMenu",
                table: "SiteNavTemplates");

            migrationBuilder.RenameColumn(
                name: "SiteSettingsLink",
                table: "SiteNavTemplates",
                newName: "SiteOptionsLink");
        }
    }
}
