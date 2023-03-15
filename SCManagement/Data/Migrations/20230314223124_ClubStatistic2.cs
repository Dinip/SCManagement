using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubStatistic2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubPaymentStatistic_Product_ProductId",
                table: "ClubPaymentStatistic");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ClubPaymentStatistic",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "ClubPaymentStatistic",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubPaymentStatistic_Product_ProductId",
                table: "ClubPaymentStatistic",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubPaymentStatistic_Product_ProductId",
                table: "ClubPaymentStatistic");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "ClubPaymentStatistic");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ClubPaymentStatistic",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubPaymentStatistic_Product_ProductId",
                table: "ClubPaymentStatistic",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
