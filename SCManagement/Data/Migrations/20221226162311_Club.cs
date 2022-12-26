using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class Club : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Club",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhotographyId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Club_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Club_BlobDto_PhotographyId",
                        column: x => x.PhotographyId,
                        principalTable: "BlobDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Modality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modality", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modality_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Modality",
                columns: new[] { "Id", "ClubId", "Name" },
                values: new object[,]
                {
                    { 1, null, "Atletismo" },
                    { 2, null, "Basquetebol" },
                    { 3, null, "Futebol" },
                    { 4, null, "Futsal" },
                    { 5, null, "Hóquei em Patins" },
                    { 6, null, "Natação" },
                    { 7, null, "Voleibol" },
                    { 8, null, "BTT" },
                    { 9, null, "Taekwondo" },
                    { 10, null, "Orientação" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Club_AddressId",
                table: "Club",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Club_PhotographyId",
                table: "Club",
                column: "PhotographyId");

            migrationBuilder.CreateIndex(
                name: "IX_Modality_ClubId",
                table: "Modality",
                column: "ClubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modality");

            migrationBuilder.DropTable(
                name: "Club");
        }
    }
}
