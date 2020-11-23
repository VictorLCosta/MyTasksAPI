using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTasksAPI.Migrations
{
    public partial class TasksSincronization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "IdTaskApi");

            migrationBuilder.AddColumn<int>(
                name: "IdTaskApp",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "Tasks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdTaskApp",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Removed",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "IdTaskApi",
                table: "Tasks",
                newName: "Id");
        }
    }
}
