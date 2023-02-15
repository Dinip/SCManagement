using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AthleteSlots",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Club",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Club",
                type: "int",
                nullable: true);

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
                name: "IX_Club_SubscriptionId",
                table: "Club",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Club_Subscription_SubscriptionId",
                table: "Club",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Club_Subscription_SubscriptionId",
                table: "Club");

            migrationBuilder.DropTable(
                name: "ClubPaymentSettings");

            migrationBuilder.DropIndex(
                name: "IX_Club_SubscriptionId",
                table: "Club");

            migrationBuilder.DropColumn(
                name: "AthleteSlots",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Club");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Club");
        }
    }
}
