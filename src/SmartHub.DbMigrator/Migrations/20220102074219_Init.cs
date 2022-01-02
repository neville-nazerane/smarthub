using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHub.DbMigrator.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceActions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Component = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Capability = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Command = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SceneActions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SceneId = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneActions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_EventId_TimeStamp",
                table: "EventLogs",
                columns: new[] { "EventId", "TimeStamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceActions");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "SceneActions");
        }
    }
}
