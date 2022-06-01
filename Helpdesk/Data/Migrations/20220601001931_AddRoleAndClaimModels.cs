using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddRoleAndClaimModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HelpdeskClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpdeskClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelpdeskRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpdeskRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelpdeskClaimHelpdeskRole",
                columns: table => new
                {
                    ClaimsId = table.Column<int>(type: "int", nullable: false),
                    GrantingRolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpdeskClaimHelpdeskRole", x => new { x.ClaimsId, x.GrantingRolesId });
                    table.ForeignKey(
                        name: "FK_HelpdeskClaimHelpdeskRole_HelpdeskClaims_ClaimsId",
                        column: x => x.ClaimsId,
                        principalTable: "HelpdeskClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpdeskClaimHelpdeskRole_HelpdeskRoles_GrantingRolesId",
                        column: x => x.GrantingRolesId,
                        principalTable: "HelpdeskRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HelpdeskRoleHelpdeskUser",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpdeskRoleHelpdeskUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_HelpdeskRoleHelpdeskUser_HelpdeskRoles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "HelpdeskRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpdeskRoleHelpdeskUser_HelpdeskUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskClaimHelpdeskRole_GrantingRolesId",
                table: "HelpdeskClaimHelpdeskRole",
                column: "GrantingRolesId");

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskRoleHelpdeskUser_UsersId",
                table: "HelpdeskRoleHelpdeskUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HelpdeskClaimHelpdeskRole");

            migrationBuilder.DropTable(
                name: "HelpdeskRoleHelpdeskUser");

            migrationBuilder.DropTable(
                name: "HelpdeskClaims");

            migrationBuilder.DropTable(
                name: "HelpdeskRoles");
        }
    }
}
