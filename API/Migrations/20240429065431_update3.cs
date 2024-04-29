using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Plant_Plant_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_User_User_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Plant_PlantNameId",
                table: "Plants");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropIndex(
                name: "IX_Plants_PlantNameId",
                table: "Plants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MoistureLevel",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "PlantNameId",
                table: "Plants");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "Plants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PlantOverviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlantNameId = table.Column<int>(type: "integer", nullable: false),
                    MoistureLevel = table.Column<int>(type: "integer", nullable: false),
                    MinWaterLevel = table.Column<int>(type: "integer", nullable: false),
                    MaxWaterLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantOverviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantOverviews_Plants_PlantNameId",
                        column: x => x.PlantNameId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantOverviews_PlantNameId",
                table: "PlantOverviews",
                column: "PlantNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Plants_Plant_idId",
                table: "PlantSensor",
                column: "Plant_idId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSensor_Users_User_idId",
                table: "PlantSensor",
                column: "User_idId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Plants_Plant_idId",
                table: "PlantSensor");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantSensor_Users_User_idId",
                table: "PlantSensor");

            migrationBuilder.DropTable(
                name: "PlantOverviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "Plants");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.AddColumn<int>(
                name: "MoistureLevel",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlantNameId",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxWaterLevel = table.Column<int>(type: "integer", nullable: false),
                    MinWaterLevel = table.Column<int>(type: "integer", nullable: false),
                    PlantName = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.Id);
                });

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
                name: "FK_PlantSensor_User_User_idId",
                table: "PlantSensor",
                column: "User_idId",
                principalTable: "User",
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
    }
}
