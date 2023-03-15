using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ClubStatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubEventStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    StatisticRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubEventStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubEventStatistic_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ClubEventStatistic_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ClubModalityStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    ModalityId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    StatisticRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubModalityStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubModalityStatistic_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubModalityStatistic_Modality_ModalityId",
                        column: x => x.ModalityId,
                        principalTable: "Modality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubPaymentStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<float>(type: "real", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StatisticRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubPaymentStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubPaymentStatistic_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubPaymentStatistic_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubUserStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    StatisticRange = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubUserStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubUserStatistic_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubUserStatistic_RoleClub_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleClub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubEventStatistic_ClubId",
                table: "ClubEventStatistic",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubEventStatistic_EventId",
                table: "ClubEventStatistic",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubModalityStatistic_ClubId",
                table: "ClubModalityStatistic",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubModalityStatistic_ModalityId",
                table: "ClubModalityStatistic",
                column: "ModalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubPaymentStatistic_ClubId",
                table: "ClubPaymentStatistic",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubPaymentStatistic_ProductId",
                table: "ClubPaymentStatistic",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubUserStatistic_ClubId",
                table: "ClubUserStatistic",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubUserStatistic_RoleId",
                table: "ClubUserStatistic",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubEventStatistic");

            migrationBuilder.DropTable(
                name: "ClubModalityStatistic");

            migrationBuilder.DropTable(
                name: "ClubPaymentStatistic");

            migrationBuilder.DropTable(
                name: "ClubUserStatistic");
        }
    }
}
