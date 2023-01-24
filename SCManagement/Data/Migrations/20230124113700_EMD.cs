using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class EMD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EMDId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EMDId",
                table: "AspNetUsers",
                column: "EMDId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BlobDto_EMDId",
                table: "AspNetUsers",
                column: "EMDId",
                principalTable: "BlobDto",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BlobDto_EMDId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EMDId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EMDId",
                table: "AspNetUsers");
        }
    }
}
