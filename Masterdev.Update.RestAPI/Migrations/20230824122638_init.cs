using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masterdev.Update.RestAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_app",
                table: "app");

            migrationBuilder.RenameTable(
                name: "app",
                newName: "apps");

            migrationBuilder.RenameColumn(
                name: "appid",
                table: "data",
                newName: "app_id");

            migrationBuilder.RenameColumn(
                name: "appname",
                table: "apps",
                newName: "App_Name");

            migrationBuilder.AddColumn<int>(
                name: "app_id",
                table: "files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_apps",
                table: "apps",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_apps",
                table: "apps");

            migrationBuilder.DropColumn(
                name: "app_id",
                table: "files");

            migrationBuilder.RenameTable(
                name: "apps",
                newName: "app");

            migrationBuilder.RenameColumn(
                name: "app_id",
                table: "data",
                newName: "appid");

            migrationBuilder.RenameColumn(
                name: "App_Name",
                table: "app",
                newName: "appname");

            migrationBuilder.AddPrimaryKey(
                name: "PK_app",
                table: "app",
                column: "id");
        }
    }
}
