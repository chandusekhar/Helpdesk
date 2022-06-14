using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddTicketsAndActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    CreationClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketActionTypes",
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
                    table.PrimaryKey("PK_TicketActionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketActionTypes_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketMasters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketTypeId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    HandlerId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TicketStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMasters_HelpdeskUsers_HandlerId",
                        column: x => x.HandlerId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketMasters_HelpdeskUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketMasters_TicketStatuses_TicketStatusId",
                        column: x => x.TicketStatusId,
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketMasters_TicketTypes_TicketTypeId",
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
                    TicketMasterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActionStatusId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "TicketWatchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EmailFollowUp = table.Column<bool>(type: "bit", nullable: false),
                    TicketMasterId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketWatchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketWatchers_HelpdeskUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketWatchers_TicketMasters_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TicketMasters",
                        principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_TicketMasters_HandlerId",
                table: "TicketMasters",
                column: "HandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMasters_RequesterId",
                table: "TicketMasters",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMasters_TicketStatusId",
                table: "TicketMasters",
                column: "TicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMasters_TicketTypeId",
                table: "TicketMasters",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketWatchers_TicketMasterId",
                table: "TicketWatchers",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketWatchers_UserId",
                table: "TicketWatchers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketActions");

            migrationBuilder.DropTable(
                name: "TicketActionTypes");

            migrationBuilder.DropTable(
                name: "TicketWatchers");

            migrationBuilder.DropTable(
                name: "ActionStatuses");

            migrationBuilder.DropTable(
                name: "TicketMasters");

            migrationBuilder.DropTable(
                name: "TicketStatuses");

            migrationBuilder.DropTable(
                name: "TicketTypes");
        }
    }
}
