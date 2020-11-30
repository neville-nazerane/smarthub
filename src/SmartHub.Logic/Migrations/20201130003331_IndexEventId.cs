using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHub.Logic.Migrations
{
    public partial class IndexEventId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "EventLogs",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_EventId",
                table: "EventLogs",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLogs_EventId",
                table: "EventLogs");

            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "EventLogs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");
        }
    }
}
