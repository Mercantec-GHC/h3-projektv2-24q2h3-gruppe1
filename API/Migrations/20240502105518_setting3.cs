using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class setting3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Setting");

            migrationBuilder.AddColumn<bool>(
                name: "AutoMode",
                table: "Setting",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoMode",
                table: "Setting");

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Setting",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
