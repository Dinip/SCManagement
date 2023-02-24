using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubRoleStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UsersRoleClub",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE UsersRoleClub SET Status = 1 WHERE Status IS NULL AND RoleId = 10");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UsersRoleClub");
        }
    }
}
