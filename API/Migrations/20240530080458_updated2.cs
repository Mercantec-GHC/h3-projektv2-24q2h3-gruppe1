using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sensor1Name",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "Sensor2Name",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "SensorID1",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "SensorID2",
                table: "Setting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sensor1Name",
                table: "Setting",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sensor2Name",
                table: "Setting",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SensorID1",
                table: "Setting",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SensorID2",
                table: "Setting",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
