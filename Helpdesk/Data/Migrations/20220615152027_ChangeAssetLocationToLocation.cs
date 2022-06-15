using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Data.Migrations
{
    public partial class ChangeAssetLocationToLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "AssetLocations");

            migrationBuilder.RenameColumn(
                name: "AssetLocationId",
                table: "Assets",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetLocationId",
                table: "Assets",
                newName: "IX_Assets_LocationId");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Assets",
                newName: "AssetLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_LocationId",
                table: "Assets",
                newName: "IX_Assets_AssetLocationId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets",
                column: "AssetLocationId",
                principalTable: "AssetLocations",
                principalColumn: "Id");
        }
    }
}
