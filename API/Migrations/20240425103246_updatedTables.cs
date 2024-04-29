using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "Sensor",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlantOverview_idId",
                table: "PlantSensor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sensor_idId",
                table: "PlantSensor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "User_idId",
                table: "PlantSensor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlantSensor_PlantOverview_idId",
                table: "PlantSensor",
                column: "PlantOverview_idId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantSensor_Sensor_idId",
                table: "PlantSensor",
                column: "Sensor_idId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantSensor_User_idId",
                table: "PlantSensor",
                column: "User_idId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Plants_PlantOverview_idId",
                table: "PlantSensor",
                column: "PlantOverview_idId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Sensor_Sensor_idId",
                table: "PlantSensor",
                column: "Sensor_idId",
                principalTable: "Sensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_User_User_idId",
                table: "PlantSensor",
                column: "User_idId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Plants_PlantOverview_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Sensor_Sensor_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_User_User_idId",
                table: "PlantSensor");

            migrationBuilder.DropIndex(
                name: "IX_PlantSensor_PlantOverview_idId",
                table: "PlantSensor");

            migrationBuilder.DropIndex(
                name: "IX_PlantSensor_Sensor_idId",
                table: "PlantSensor");

            migrationBuilder.DropIndex(
                name: "IX_PlantSensor_User_idId",
                table: "PlantSensor");

            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "PlantOverview_idId",
                table: "PlantSensor");

            migrationBuilder.DropColumn(
                name: "Sensor_idId",
                table: "PlantSensor");

            migrationBuilder.DropColumn(
                name: "User_idId",
                table: "PlantSensor");
        }
    }
}
