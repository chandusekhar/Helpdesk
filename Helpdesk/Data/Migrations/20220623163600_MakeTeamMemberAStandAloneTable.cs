using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class MakeTeamMemberAStandAloneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_HelpdeskUserId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_HelpdeskUserId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "HelpdeskUserId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Subordinate",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "TeamMembers");

            migrationBuilder.AddColumn<int>(
                name: "SubordinateId",
                table: "TeamMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TeamMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_SubordinateId",
                table: "TeamMembers",
                column: "SubordinateId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_SupervisorId",
                table: "TeamMembers",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_SubordinateId",
                table: "TeamMembers",
                column: "SubordinateId",
                principalTable: "HelpdeskUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_SupervisorId",
                table: "TeamMembers",
                column: "SupervisorId",
                principalTable: "HelpdeskUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_SubordinateId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_SupervisorId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_SubordinateId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_SupervisorId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "SubordinateId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TeamMembers");

            migrationBuilder.AddColumn<int>(
                name: "HelpdeskUserId",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subordinate",
                table: "TeamMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "TeamMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_HelpdeskUserId",
                table: "TeamMembers",
                column: "HelpdeskUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_HelpdeskUsers_HelpdeskUserId",
                table: "TeamMembers",
                column: "HelpdeskUserId",
                principalTable: "HelpdeskUsers",
                principalColumn: "Id");
        }
    }
}
