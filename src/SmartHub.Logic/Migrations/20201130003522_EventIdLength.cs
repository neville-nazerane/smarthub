using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartHub.Logic.Migrations
{
    public partial class EventIdLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SceneId",
                table: "SceneActions",
                type: "varchar(90) CHARACTER SET utf8mb4",
                maxLength: 90,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "EventLogs",
                type: "varchar(90) CHARACTER SET utf8mb4",
                maxLength: 90,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SceneId",
                table: "SceneActions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(90) CHARACTER SET utf8mb4",
                oldMaxLength: 90);

            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "EventLogs",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(90) CHARACTER SET utf8mb4",
                oldMaxLength: 90);
        }
    }
}
