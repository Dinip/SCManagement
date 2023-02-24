using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubPaymentsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE ClubPaymentSettings SET QuotaFrequency = 0 WHERE QuotaFrequency IS NULL");
            migrationBuilder.Sql("UPDATE ClubPaymentSettings SET QuotaFee = 0 WHERE QuotaFee IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "QuotaFrequency",
                table: "ClubPaymentSettings",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "QuotaFee",
                table: "ClubPaymentSettings",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ValidCredentials",
                table: "ClubPaymentSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE ClubPaymentSettings SET ValidCredentials = 0 WHERE AccountId IS NULL AND AccountKey IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidCredentials",
                table: "ClubPaymentSettings");

            migrationBuilder.AlterColumn<int>(
                name: "QuotaFrequency",
                table: "ClubPaymentSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "QuotaFee",
                table: "ClubPaymentSettings",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
