using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class Bioimpedance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bioimpedance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: true),
                    FatMass = table.Column<double>(type: "float", nullable: true),
                    LeanMass = table.Column<double>(type: "float", nullable: true),
                    MuscleMass = table.Column<double>(type: "float", nullable: true),
                    ViceralFat = table.Column<double>(type: "float", nullable: true),
                    BasalMetabolism = table.Column<double>(type: "float", nullable: true),
                    Hydration = table.Column<double>(type: "float", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bioimpedance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bioimpedance_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bioimpedance_UserId",
                table: "Bioimpedance",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bioimpedance");
        }
    }
}
