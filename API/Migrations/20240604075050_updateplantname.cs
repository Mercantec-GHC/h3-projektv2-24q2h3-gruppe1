using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updateplantname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlantNameId",
                table: "PlantOverviews");

            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "PlantOverviews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "PlantOverviews");

            migrationBuilder.AddColumn<int>(
                name: "PlantNameId",
                table: "PlantOverviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
