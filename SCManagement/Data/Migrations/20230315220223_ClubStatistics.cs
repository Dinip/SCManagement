using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubModalityStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    ModalityId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    StatisticsRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubModalityStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubModalityStatistics_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubModalityStatistics_Modality_ModalityId",
                        column: x => x.ModalityId,
                        principalTable: "Modality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubPaymentStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<float>(type: "real", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    StatisticsRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubPaymentStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubPaymentStatistics_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubPaymentStatistics_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClubUserStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    StatisticsRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubUserStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubUserStatistics_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubUserStatistics_RoleClub_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleClub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubModalityStatistics_ClubId",
                table: "ClubModalityStatistics",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubModalityStatistics_ModalityId",
                table: "ClubModalityStatistics",
                column: "ModalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubPaymentStatistics_ClubId",
                table: "ClubPaymentStatistics",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubPaymentStatistics_ProductId",
                table: "ClubPaymentStatistics",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubUserStatistics_ClubId",
                table: "ClubUserStatistics",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubUserStatistics_RoleId",
                table: "ClubUserStatistics",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubModalityStatistics");

            migrationBuilder.DropTable(
                name: "ClubPaymentStatistics");

            migrationBuilder.DropTable(
                name: "ClubUserStatistics");
        }
    }
}
