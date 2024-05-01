using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class upduserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Plants",
                newName: "UserIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserIdId",
                table: "Plants",
                column: "UserIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Users_UserIdId",
                table: "Plants",
                column: "UserIdId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Users_UserIdId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_UserIdId",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "UserIdId",
                table: "Plants",
                newName: "UserId");
        }
    }
}
