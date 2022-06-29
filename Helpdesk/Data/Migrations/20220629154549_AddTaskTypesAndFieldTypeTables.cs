using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddTaskTypesAndFieldTypeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketTaskTypes");

            migrationBuilder.CreateTable(
                name: "FieldDataBooleans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataBooleans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDataDatetimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataDatetimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDataEntityIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataEntityIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDataIntegers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataIntegers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDataNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(28,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDataStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDataStrings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTypes_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowNulls = table.Column<bool>(type: "bit", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    ReferenceTargetType = table.Column<int>(type: "int", nullable: false),
                    ReferenceDefinitionField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldTypes_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_TaskTypeId",
                table: "FieldTypes",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypes_TicketTypeId",
                table: "TaskTypes",
                column: "TicketTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldDataBooleans");

            migrationBuilder.DropTable(
                name: "FieldDataDatetimes");

            migrationBuilder.DropTable(
                name: "FieldDataEntityIds");

            migrationBuilder.DropTable(
                name: "FieldDataIntegers");

            migrationBuilder.DropTable(
                name: "FieldDataNumbers");

            migrationBuilder.DropTable(
                name: "FieldDataStrings");

            migrationBuilder.DropTable(
                name: "FieldTypes");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.CreateTable(
                name: "TicketTaskTypes",
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
                    table.PrimaryKey("PK_TicketTaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTaskTypes_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTaskTypes_TicketTypeId",
                table: "TicketTaskTypes",
                column: "TicketTypeId");
        }
    }
}
