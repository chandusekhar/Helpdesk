using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class AddInitialAssetTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                table: "FileUploads",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "FileUploads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                table: "AssetLicenseAssignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AssetLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    AssetTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetModels_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetModels_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatingUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssetTypeId = table.Column<int>(type: "int", nullable: true),
                    AssetStatusId = table.Column<int>(type: "int", nullable: true),
                    AssetLocationId = table.Column<int>(type: "int", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    AssetModelId = table.Column<int>(type: "int", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true),
                    PurchasingVendorId = table.Column<int>(type: "int", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuoteNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasingUserId = table.Column<int>(type: "int", nullable: true),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovingUserId = table.Column<int>(type: "int", nullable: true),
                    WarrantyInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetLocations_AssetLocationId",
                        column: x => x.AssetLocationId,
                        principalTable: "AssetLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_AssetStatuses_AssetStatusId",
                        column: x => x.AssetStatusId,
                        principalTable: "AssetStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_HelpdeskUsers_ApprovingUserId",
                        column: x => x.ApprovingUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_HelpdeskUsers_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_HelpdeskUsers_CreatingUserId",
                        column: x => x.CreatingUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_HelpdeskUsers_PurchasingUserId",
                        column: x => x.PurchasingUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Vendors_PurchasingVendorId",
                        column: x => x.PurchasingVendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EditingUserId = table.Column<int>(type: "int", nullable: false),
                    WhenEdited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetChangeLogs_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetChangeLogs_HelpdeskUsers_EditingUserId",
                        column: x => x.EditingUserId,
                        principalTable: "HelpdeskUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_AssetId",
                table: "FileUploads",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_DocumentTypeId",
                table: "FileUploads",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLicenseAssignments_AssetId",
                table: "AssetLicenseAssignments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeLogs_AssetId",
                table: "AssetChangeLogs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeLogs_EditingUserId",
                table: "AssetChangeLogs",
                column: "EditingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_AssetTypeId",
                table: "AssetModels",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_ManufacturerId",
                table: "AssetModels",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ApprovingUserId",
                table: "Assets",
                column: "ApprovingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetLocationId",
                table: "Assets",
                column: "AssetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetModelId",
                table: "Assets",
                column: "AssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetStatusId",
                table: "Assets",
                column: "AssetStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssignedUserId",
                table: "Assets",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatingUserId",
                table: "Assets",
                column: "CreatingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_GroupId",
                table: "Assets",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ManufacturerId",
                table: "Assets",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchasingUserId",
                table: "Assets",
                column: "PurchasingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchasingVendorId",
                table: "Assets",
                column: "PurchasingVendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLicenseAssignments_Assets_AssetId",
                table: "AssetLicenseAssignments",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_Assets_AssetId",
                table: "FileUploads",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_DocumentTypes_DocumentTypeId",
                table: "FileUploads",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLicenseAssignments_Assets_AssetId",
                table: "AssetLicenseAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_Assets_AssetId",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_DocumentTypes_DocumentTypeId",
                table: "FileUploads");

            migrationBuilder.DropTable(
                name: "AssetChangeLogs");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "AssetLocations");

            migrationBuilder.DropTable(
                name: "AssetModels");

            migrationBuilder.DropTable(
                name: "AssetStatuses");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_AssetId",
                table: "FileUploads");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_DocumentTypeId",
                table: "FileUploads");

            migrationBuilder.DropIndex(
                name: "IX_AssetLicenseAssignments_AssetId",
                table: "AssetLicenseAssignments");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "AssetLicenseAssignments");
        }
    }
}
