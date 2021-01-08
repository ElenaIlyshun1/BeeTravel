using Microsoft.EntityFrameworkCore.Migrations;

namespace BeeTravel.Migrations
{
    public partial class CreateRoleColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleColor",
                table: "AspNetRoles",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleColor",
                table: "AspNetRoles");
        }
    }
}
