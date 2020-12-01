using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHub.Logic.Migrations
{
    public partial class LogDateIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLogs_EventId",
                table: "EventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_EventId_TimeStamp",
                table: "EventLogs",
                columns: new[] { "EventId", "TimeStamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLogs_EventId_TimeStamp",
                table: "EventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_EventId",
                table: "EventLogs",
                column: "EventId");
        }
    }
}
