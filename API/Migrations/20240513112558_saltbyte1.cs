using Microsoft.EntityFrameworkCore.Migrations;
using System.Net.NetworkInformation;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class saltbyte1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "Users",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Salt",
                table: "Users",
                type: "smallint",
            nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea");
        }
    }
}
