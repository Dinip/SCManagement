using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class BioimpedanceHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Bioimpedance",
                table: "Bioimpedance");

            migrationBuilder.RenameColumn(
                name: "BioimpedanceId",
                table: "Bioimpedance",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Bioimpedance",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bioimpedance",
                table: "Bioimpedance",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bioimpedance_UserId",
                table: "Bioimpedance",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bioimpedance_AspNetUsers_UserId",
                table: "Bioimpedance",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bioimpedance_AspNetUsers_UserId",
                table: "Bioimpedance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bioimpedance",
                table: "Bioimpedance");

            migrationBuilder.DropIndex(
                name: "IX_Bioimpedance_UserId",
                table: "Bioimpedance");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Bioimpedance");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bioimpedance",
                newName: "BioimpedanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bioimpedance",
                table: "Bioimpedance",
                column: "BioimpedanceId");
        }
    }
}
