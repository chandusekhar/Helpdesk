using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class ChangeActionToTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketActions");

            migrationBuilder.DropTable(
                name: "TicketActionTypes");

            migrationBuilder.DropTable(
                name: "ActionStatuses");

            migrationBuilder.RenameColumn(
                name: "ActionStatusesLink",
                table: "SiteNavTemplates",
                newName: "TaskStatusesLink");

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketTaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    CreationClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTaskTypes_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketMasterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTasks_TaskStatuses_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalTable: "TaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketTasks_TicketMasters_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TicketMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TaskStatusId",
                table: "TicketTasks",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TicketMasterId",
                table: "TicketTasks",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTaskTypes_TicketTypeId",
                table: "TicketTaskTypes",
                column: "TicketTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketTasks");

            migrationBuilder.DropTable(
                name: "TicketTaskTypes");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

            migrationBuilder.RenameColumn(
                name: "TaskStatusesLink",
                table: "SiteNavTemplates",
                newName: "ActionStatusesLink");

            migrationBuilder.CreateTable(
                name: "ActionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EditClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketTypeId = table.Column<int>(type: "int", nullable: true),
                    ViewClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketActionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketActionTypes_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionStatusId = table.Column<int>(type: "int", nullable: false),
                    TicketMasterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketActions_ActionStatuses_ActionStatusId",
                        column: x => x.ActionStatusId,
                        principalTable: "ActionStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketActions_TicketMasters_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TicketMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketActions_ActionStatusId",
                table: "TicketActions",
                column: "ActionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketActions_TicketMasterId",
                table: "TicketActions",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketActionTypes_TicketTypeId",
                table: "TicketActionTypes",
                column: "TicketTypeId");
        }
    }
}
