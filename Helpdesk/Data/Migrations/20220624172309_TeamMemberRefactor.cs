using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class TeamMemberRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorResponsibilities_TeamMembers_TeamMemberId",
                table: "SupervisorResponsibilities");

            migrationBuilder.DropIndex(
                name: "IX_SupervisorResponsibilities_TeamMemberId",
                table: "SupervisorResponsibilities");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "SupervisorResponsibilities");

            migrationBuilder.CreateTable(
                name: "TeamMemberResponsibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamMemberId = table.Column<int>(type: "int", nullable: false),
                    ResponsibilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMemberResponsibilities_SupervisorResponsibilities_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalTable: "SupervisorResponsibilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMemberResponsibilities_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberResponsibilities_ResponsibilityId",
                table: "TeamMemberResponsibilities",
                column: "ResponsibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberResponsibilities_TeamMemberId",
                table: "TeamMemberResponsibilities",
                column: "TeamMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMemberResponsibilities");

            migrationBuilder.AddColumn<int>(
                name: "TeamMemberId",
                table: "SupervisorResponsibilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorResponsibilities_TeamMemberId",
                table: "SupervisorResponsibilities",
                column: "TeamMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisorResponsibilities_TeamMembers_TeamMemberId",
                table: "SupervisorResponsibilities",
                column: "TeamMemberId",
                principalTable: "TeamMembers",
                principalColumn: "Id");
        }
    }
}
