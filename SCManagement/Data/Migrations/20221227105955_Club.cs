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
                name: "Modalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modalities_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModalitiesClubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    ModalityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalitiesClubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModalitiesClubs_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModalitiesClubs_Modalities_ModalityId",
                        column: x => x.ModalityId,
                        principalTable: "Modalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modalities",
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
                name: "IX_Modalities_ClubId",
                table: "Modalities",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ModalitiesClubs_ClubId",
                table: "ModalitiesClubs",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ModalitiesClubs_ModalityId",
                table: "ModalitiesClubs",
                column: "ModalityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModalitiesClubs");

            migrationBuilder.DropTable(
                name: "RolesClub");

            migrationBuilder.DropTable(
                name: "Modalities");

            migrationBuilder.DropTable(
                name: "Club");
        }
    }
}
