using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfiolioMVC.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "settingsNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settingsNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "viewerCountings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    IsCurrentViewing = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstViewing = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastViewing = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_viewerCountings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewerLoggin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ViewerCountingId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientIp = table.Column<string>(type: "TEXT", nullable: false),
                    LogginTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note1 = table.Column<string>(type: "TEXT", nullable: false),
                    Note2 = table.Column<string>(type: "TEXT", nullable: false),
                    Note3 = table.Column<string>(type: "TEXT", nullable: false),
                    userId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewerLoggin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewerLoggin_viewerCountings_ViewerCountingId",
                        column: x => x.ViewerCountingId,
                        principalTable: "viewerCountings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_settingsNumbers_Key",
                table: "settingsNumbers",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_viewerCountings_ClientId",
                table: "viewerCountings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewerLoggin_ViewerCountingId",
                table: "ViewerLoggin",
                column: "ViewerCountingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settingsNumbers");

            migrationBuilder.DropTable(
                name: "ViewerLoggin");

            migrationBuilder.DropTable(
                name: "viewerCountings");
        }
    }
}
