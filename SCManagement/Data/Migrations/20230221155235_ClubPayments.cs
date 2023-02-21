using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfigUrl",
                table: "Subscription",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AthleteSlots",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Club",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClubPaymentSettings",
                columns: table => new
                {
                    ClubPaymentSettingsId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuotaFrequency = table.Column<int>(type: "int", nullable: true),
                    QuotaFee = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubPaymentSettings", x => x.ClubPaymentSettingsId);
                    table.ForeignKey(
                        name: "FK_ClubPaymentSettings_Club_ClubPaymentSettingsId",
                        column: x => x.ClubPaymentSettingsId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_ClubId",
                table: "Subscription",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Club_ClubId",
                table: "Subscription",
                column: "ClubId",
                principalTable: "Club",
                principalColumn: "Id");

            migrationBuilder.Sql("INSERT INTO ClubPaymentSettings (ClubPaymentSettingsId, RequestSecret) SELECT Id, LOWER(NEWID()) FROM dbo.Club");
            migrationBuilder.Sql("INSERT INTO Product (Name, Value, ProductType, IsSubscription, Frequency, Enabled, AthleteSlots) VALUES ('Club Plan XS', 30, 3, 1, 3, 1, 25)");
            migrationBuilder.Sql("INSERT INTO Product (Name, Value, ProductType, IsSubscription, Frequency, Enabled, AthleteSlots) VALUES ('Club Plan S', 50, 3, 1, 3, 1, 40)");
            migrationBuilder.Sql("INSERT INTO Product (Name, Value, ProductType, IsSubscription, Frequency, Enabled, AthleteSlots) VALUES ('Club Plan M', 80, 3, 1, 3, 1, 100)");
            migrationBuilder.Sql("INSERT INTO Product (Name, Value, ProductType, IsSubscription, Frequency, Enabled, AthleteSlots) VALUES ('Club Plan L', 150, 3, 1, 3, 1, 250)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Club_ClubId",
                table: "Subscription");

            migrationBuilder.DropTable(
                name: "ClubPaymentSettings");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_ClubId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "ConfigUrl",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "AthleteSlots",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Club");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
