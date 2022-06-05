using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class MakeGroupOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpdeskUsers_Groups_GroupsId",
                table: "HelpdeskUsers");

            migrationBuilder.DropIndex(
                name: "IX_HelpdeskUsers_GroupsId",
                table: "HelpdeskUsers");

            migrationBuilder.DropColumn(
                name: "GroupsId",
                table: "HelpdeskUsers");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "HelpdeskUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskUsers_GroupId",
                table: "HelpdeskUsers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpdeskUsers_Groups_GroupId",
                table: "HelpdeskUsers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpdeskUsers_Groups_GroupId",
                table: "HelpdeskUsers");

            migrationBuilder.DropIndex(
                name: "IX_HelpdeskUsers_GroupId",
                table: "HelpdeskUsers");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "HelpdeskUsers");

            migrationBuilder.AddColumn<int>(
                name: "GroupsId",
                table: "HelpdeskUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskUsers_GroupsId",
                table: "HelpdeskUsers",
                column: "GroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpdeskUsers_Groups_GroupsId",
                table: "HelpdeskUsers",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
