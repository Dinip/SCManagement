using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class BioImpedance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bioimpedance",
                columns: table => new
                {
                    BioimpedanceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatMass = table.Column<float>(type: "real", nullable: true),
                    LeanMass = table.Column<float>(type: "real", nullable: true),
                    MuscleMass = table.Column<float>(type: "real", nullable: true),
                    ViceralFat = table.Column<float>(type: "real", nullable: true),
                    BasalMetabolism = table.Column<float>(type: "real", nullable: true),
                    Hydration = table.Column<float>(type: "real", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bioimpedance", x => x.BioimpedanceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bioimpedance");
        }
    }
}
