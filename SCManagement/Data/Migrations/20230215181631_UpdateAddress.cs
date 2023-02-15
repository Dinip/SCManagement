using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class UpdateAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_County_CountyId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_CountyId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CoordinateX",
                table: "Address",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CoordinateY",
                table: "Address",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CoordinateX",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CoordinateY",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Address");

            migrationBuilder.AddColumn<int>(
                name: "CountyId",
                table: "Address",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountyId",
                table: "Address",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_County_CountyId",
                table: "Address",
                column: "CountyId",
                principalTable: "County",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
