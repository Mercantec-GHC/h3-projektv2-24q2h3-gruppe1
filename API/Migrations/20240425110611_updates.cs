using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Plants_PlantOverview_idId",
                table: "PlantSensor");

            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "PlantOverview_idId",
                table: "PlantSensor",
                newName: "Plant_idId");

            migrationBuilder.RenameIndex(
                name: "IX_PlantSensor_PlantOverview_idId",
                table: "PlantSensor",
                newName: "IX_PlantSensor_Plant_idId");

            migrationBuilder.AddColumn<int>(
                name: "PlantNameId",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_PlantNameId",
                table: "Plants",
                column: "PlantNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Plant_Plant_idId",
                table: "PlantSensor",
                column: "Plant_idId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Plant_PlantNameId",
                table: "Plants",
                column: "PlantNameId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Plant_Plant_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Plant_PlantNameId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_PlantNameId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "PlantNameId",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "Plant_idId",
                table: "PlantSensor",
                newName: "PlantOverview_idId");

            migrationBuilder.RenameIndex(
                name: "IX_PlantSensor_Plant_idId",
                table: "PlantSensor",
                newName: "IX_PlantSensor_PlantOverview_idId");

            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "Plants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Plants_PlantOverview_idId",
                table: "PlantSensor",
                column: "PlantOverview_idId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
