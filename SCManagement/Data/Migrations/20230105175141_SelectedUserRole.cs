using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class SelectedUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Selected",
                table: "UsersRoleClub",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Selected",
                table: "UsersRoleClub");
        }
    }
}
