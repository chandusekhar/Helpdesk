using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddTicketPrioritySystemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemType",
                table: "TicketPriority",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemType",
                table: "TicketPriority");
        }
    }
}
