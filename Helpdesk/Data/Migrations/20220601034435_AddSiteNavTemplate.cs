using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddSiteNavTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiteNavTemplateId",
                table: "HelpdeskUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SiteNavTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketLink = table.Column<bool>(type: "bit", nullable: false),
                    AssetLink = table.Column<bool>(type: "bit", nullable: false),
                    PeopleLink = table.Column<bool>(type: "bit", nullable: false),
                    SiteOptionsLink = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNavTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskUsers_SiteNavTemplateId",
                table: "HelpdeskUsers",
                column: "SiteNavTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpdeskUsers_SiteNavTemplates_SiteNavTemplateId",
                table: "HelpdeskUsers",
                column: "SiteNavTemplateId",
                principalTable: "SiteNavTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpdeskUsers_SiteNavTemplates_SiteNavTemplateId",
                table: "HelpdeskUsers");

            migrationBuilder.DropTable(
                name: "SiteNavTemplates");

            migrationBuilder.DropIndex(
                name: "IX_HelpdeskUsers_SiteNavTemplateId",
                table: "HelpdeskUsers");

            migrationBuilder.DropColumn(
                name: "SiteNavTemplateId",
                table: "HelpdeskUsers");
        }
    }
}
