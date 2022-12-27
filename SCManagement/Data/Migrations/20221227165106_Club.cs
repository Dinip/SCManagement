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
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhotographyId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Club_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Club_BlobDto_PhotographyId",
                        column: x => x.PhotographyId,
                        principalTable: "BlobDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Modalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesClub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesClub", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubModality",
                columns: table => new
                {
                    ClubsId = table.Column<int>(type: "int", nullable: false),
                    ModalitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubModality", x => new { x.ClubsId, x.ModalitiesId });
                    table.ForeignKey(
                        name: "FK_ClubModality_Club_ClubsId",
                        column: x => x.ClubsId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubModality_Modalities_ModalitiesId",
                        column: x => x.ModalitiesId,
                        principalTable: "Modalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modalities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Atletismo" },
                    { 2, "Basquetebol" },
                    { 3, "Futebol" },
                    { 4, "Futsal" },
                    { 5, "Hóquei em Patins" },
                    { 6, "Natação" },
                    { 7, "Voleibol" },
                    { 8, "BTT" },
                    { 9, "Taekwondo" },
                    { 10, "Orientação" }
                });

            migrationBuilder.InsertData(
                table: "RolesClub",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Sócio" },
                    { 2, "Atleta" },
                    { 3, "Treinador" },
                    { 4, "Secretaria" },
                    { 5, "Administrador de Clube" }
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
                name: "IX_ClubModality_ModalitiesId",
                table: "ClubModality",
                column: "ModalitiesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubModality");

            migrationBuilder.DropTable(
                name: "RolesClub");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.DropTable(
                name: "Modalities");
        }
    }
}
