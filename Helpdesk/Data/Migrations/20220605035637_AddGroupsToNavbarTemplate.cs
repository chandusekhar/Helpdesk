using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddGroupsToNavbarTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupsLink",
                table: "SiteNavTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupsLink",
                table: "SiteNavTemplates");
        }
    }
}
