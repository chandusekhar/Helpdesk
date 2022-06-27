using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddTicketPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "TicketMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketPriorityId",
                table: "TicketMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TicketPriority",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPriority", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketMasters_TicketPriorityId",
                table: "TicketMasters",
                column: "TicketPriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMasters_TicketPriority_TicketPriorityId",
                table: "TicketMasters",
                column: "TicketPriorityId",
                principalTable: "TicketPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMasters_TicketPriority_TicketPriorityId",
                table: "TicketMasters");

            migrationBuilder.DropTable(
                name: "TicketPriority");

            migrationBuilder.DropIndex(
                name: "IX_TicketMasters_TicketPriorityId",
                table: "TicketMasters");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "TicketMasters");

            migrationBuilder.DropColumn(
                name: "TicketPriorityId",
                table: "TicketMasters");
        }
    }
}
