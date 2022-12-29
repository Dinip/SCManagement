using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class clubs_modalities_roles : Migration
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
                name: "Modality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClub", x => x.Id);
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
                        name: "FK_ClubModality_Modality_ModalitiesId",
                        column: x => x.ModalitiesId,
                        principalTable: "Modality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoleClub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoleClub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRoleClub_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoleClub_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoleClub_RoleClub_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleClub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Águeda");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "Ílhavo");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "Oliveira de Azeméis");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "São João da Madeira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "Almodôvar");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "Mértola");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "Guimarães");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 42,
                column: "Name",
                value: "Póvoa de Lanhoso");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "Vila Nova de Famalicão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "Alfândega da Fé");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 49,
                column: "Name",
                value: "Bragança");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "Carrazeda de Ansiães");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 51,
                column: "Name",
                value: "Freixo de Espada à Cinta");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 62,
                column: "Name",
                value: "Covilhã");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 63,
                column: "Name",
                value: "Fundão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 67,
                column: "Name",
                value: "Proença-a-Nova");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 68,
                column: "Name",
                value: "Sertã");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 70,
                column: "Name",
                value: "Vila Velha de Ródão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 76,
                column: "Name",
                value: "Góis");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 77,
                column: "Name",
                value: "Lousã");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 86,
                column: "Name",
                value: "Tábua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 92,
                column: "Name",
                value: "Évora");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 95,
                column: "Name",
                value: "Mourão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 101,
                column: "Name",
                value: "Vila Viçosa");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 109,
                column: "Name",
                value: "Loulé");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 111,
                column: "Name",
                value: "Olhão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 112,
                column: "Name",
                value: "Portimão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 113,
                column: "Name",
                value: "São Brás de Alportel");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 117,
                column: "Name",
                value: "Vila Real de Santo António");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 126,
                column: "Name",
                value: "Mêda");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 131,
                column: "Name",
                value: "Vila Nova de Foz Côa");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 132,
                column: "Name",
                value: "Alcobaça");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 133,
                column: "Name",
                value: "Alvaiázere");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 134,
                column: "Name",
                value: "Ansião");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 138,
                column: "Name",
                value: "Castanheira de Pêra");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 139,
                column: "Name",
                value: "Figueiró dos Vinhos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 142,
                column: "Name",
                value: "Nazaré");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 143,
                column: "Name",
                value: "Óbidos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 144,
                column: "Name",
                value: "Pedrógão Grande");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 147,
                column: "Name",
                value: "Porto de Mós");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 155,
                column: "Name",
                value: "Lourinhã");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 159,
                column: "Name",
                value: "Sobral de Monte Agraço");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 164,
                column: "Name",
                value: "Alter do Chão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 172,
                column: "Name",
                value: "Gavião");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 173,
                column: "Name",
                value: "Marvão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 180,
                column: "Name",
                value: "Baião");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 187,
                column: "Name",
                value: "Paços de Ferreira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 191,
                column: "Name",
                value: "Póvoa de Varzim");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 200,
                column: "Name",
                value: "Alpiarça");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 204,
                column: "Name",
                value: "Constância");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 207,
                column: "Name",
                value: "Ferreira do Zêzere");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 208,
                column: "Name",
                value: "Golegã");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 209,
                column: "Name",
                value: "Mação");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 212,
                column: "Name",
                value: "Santarém");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 217,
                column: "Name",
                value: "Ourém");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 218,
                column: "Name",
                value: "Alcácer do Sal");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 222,
                column: "Name",
                value: "Grândola");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 226,
                column: "Name",
                value: "Santiago do Cacém");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 229,
                column: "Name",
                value: "Setúbal");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 233,
                column: "Name",
                value: "Melgaço");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 234,
                column: "Name",
                value: "Monção");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 238,
                column: "Name",
                value: "Valença");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 241,
                column: "Name",
                value: "Alijó");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 244,
                column: "Name",
                value: "Mesão Frio");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 247,
                column: "Name",
                value: "Murça");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 248,
                column: "Name",
                value: "Peso da Régua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 251,
                column: "Name",
                value: "Santa Marta de Penaguião");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 252,
                column: "Name",
                value: "Valpaços");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 258,
                column: "Name",
                value: "Cinfães");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 262,
                column: "Name",
                value: "Mortágua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 268,
                column: "Name",
                value: "Santa Comba Dão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 269,
                column: "Name",
                value: "São João da Pesqueira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 270,
                column: "Name",
                value: "São Pedro do Sul");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 271,
                column: "Name",
                value: "Sátão");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 273,
                column: "Name",
                value: "Tabuaço");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 280,
                column: "Name",
                value: "Câmara de Lobos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 288,
                column: "Name",
                value: "São Vicente");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 294,
                column: "Name",
                value: "Povoação");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 297,
                column: "Name",
                value: "Angra do Heroísmo");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 298,
                column: "Name",
                value: "Vila da Praia da Vitória");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 304,
                column: "Name",
                value: "São Roque do Pico");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Bragança");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Évora");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 14,
                column: "Name",
                value: "Santarém");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "Setúbal");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Ilha de São Miguel");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Ilha de São Jorge");

            migrationBuilder.InsertData(
                table: "Modality",
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
                table: "RoleClub",
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

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoleClub_ClubId",
                table: "UsersRoleClub",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoleClub_RoleId",
                table: "UsersRoleClub",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoleClub_UserId",
                table: "UsersRoleClub",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubModality");

            migrationBuilder.DropTable(
                name: "UsersRoleClub");

            migrationBuilder.DropTable(
                name: "Modality");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.DropTable(
                name: "RoleClub");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Ãgueda");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "Ãlhavo");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "Oliveira de AzemÃ©is");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "SÃ£o JoÃ£o da Madeira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "AlmodÃ´var");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "MÃ©rtola");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "GuimarÃ£es");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 42,
                column: "Name",
                value: "PÃ³voa de Lanhoso");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "Vila Nova de FamalicÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "AlfÃ¢ndega da FÃ©");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 49,
                column: "Name",
                value: "BraganÃ§a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "Carrazeda de AnsiÃ£es");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 51,
                column: "Name",
                value: "Freixo de Espada Ã  Cinta");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 62,
                column: "Name",
                value: "CovilhÃ£");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 63,
                column: "Name",
                value: "FundÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 67,
                column: "Name",
                value: "ProenÃ§a-a-Nova");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 68,
                column: "Name",
                value: "SertÃ£");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 70,
                column: "Name",
                value: "Vila Velha de RÃ³dÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 76,
                column: "Name",
                value: "GÃ³is");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 77,
                column: "Name",
                value: "LousÃ£");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 86,
                column: "Name",
                value: "TÃ¡bua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 92,
                column: "Name",
                value: "Ã‰vora");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 95,
                column: "Name",
                value: "MourÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 101,
                column: "Name",
                value: "Vila ViÃ§osa");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 109,
                column: "Name",
                value: "LoulÃ©");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 111,
                column: "Name",
                value: "OlhÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 112,
                column: "Name",
                value: "PortimÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 113,
                column: "Name",
                value: "SÃ£o BrÃ¡s de Alportel");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 117,
                column: "Name",
                value: "Vila Real de Santo AntÃ³nio");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 126,
                column: "Name",
                value: "MÃªda");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 131,
                column: "Name",
                value: "Vila Nova de Foz CÃ´a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 132,
                column: "Name",
                value: "AlcobaÃ§a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 133,
                column: "Name",
                value: "AlvaiÃ¡zere");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 134,
                column: "Name",
                value: "AnsiÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 138,
                column: "Name",
                value: "Castanheira de PÃªra");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 139,
                column: "Name",
                value: "FigueirÃ³ dos Vinhos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 142,
                column: "Name",
                value: "NazarÃ©");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 143,
                column: "Name",
                value: "Ã“bidos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 144,
                column: "Name",
                value: "PedrÃ³gÃ£o Grande");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 147,
                column: "Name",
                value: "Porto de MÃ³s");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 155,
                column: "Name",
                value: "LourinhÃ£");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 159,
                column: "Name",
                value: "Sobral de Monte AgraÃ§o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 164,
                column: "Name",
                value: "Alter do ChÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 172,
                column: "Name",
                value: "GaviÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 173,
                column: "Name",
                value: "MarvÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 180,
                column: "Name",
                value: "BaiÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 187,
                column: "Name",
                value: "PaÃ§os de Ferreira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 191,
                column: "Name",
                value: "PÃ³voa de Varzim");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 200,
                column: "Name",
                value: "AlpiarÃ§a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 204,
                column: "Name",
                value: "ConstÃ¢ncia");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 207,
                column: "Name",
                value: "Ferreira do ZÃªzere");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 208,
                column: "Name",
                value: "GolegÃ£");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 209,
                column: "Name",
                value: "MaÃ§Ã£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 212,
                column: "Name",
                value: "SantarÃ©m");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 217,
                column: "Name",
                value: "OurÃ©m");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 218,
                column: "Name",
                value: "AlcÃ¡cer do Sal");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 222,
                column: "Name",
                value: "GrÃ¢ndola");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 226,
                column: "Name",
                value: "Santiago do CacÃ©m");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 229,
                column: "Name",
                value: "SetÃºbal");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 233,
                column: "Name",
                value: "MelgaÃ§o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 234,
                column: "Name",
                value: "MonÃ§Ã£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 238,
                column: "Name",
                value: "ValenÃ§a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 241,
                column: "Name",
                value: "AlijÃ³");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 244,
                column: "Name",
                value: "MesÃ£o Frio");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 247,
                column: "Name",
                value: "MurÃ§a");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 248,
                column: "Name",
                value: "Peso da RÃ©gua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 251,
                column: "Name",
                value: "Santa Marta de PenaguiÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 252,
                column: "Name",
                value: "ValpaÃ§os");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 258,
                column: "Name",
                value: "CinfÃ£es");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 262,
                column: "Name",
                value: "MortÃ¡gua");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 268,
                column: "Name",
                value: "Santa Comba DÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 269,
                column: "Name",
                value: "SÃ£o JoÃ£o da Pesqueira");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 270,
                column: "Name",
                value: "SÃ£o Pedro do Sul");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 271,
                column: "Name",
                value: "SÃ¡tÃ£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 273,
                column: "Name",
                value: "TabuaÃ§o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 280,
                column: "Name",
                value: "CÃ¢mara de Lobos");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 288,
                column: "Name",
                value: "SÃ£o Vicente");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 294,
                column: "Name",
                value: "PovoaÃ§Ã£o");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 297,
                column: "Name",
                value: "Angra do HeroÃ­smo");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 298,
                column: "Name",
                value: "Vila da Praia da VitÃ³ria");

            migrationBuilder.UpdateData(
                table: "County",
                keyColumn: "Id",
                keyValue: 304,
                column: "Name",
                value: "SÃ£o Roque do Pico");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "BraganÃ§a");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Ã‰vora");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 14,
                column: "Name",
                value: "SantarÃ©m");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "SetÃºbal");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Ilha de SÃ£o Miguel");

            migrationBuilder.UpdateData(
                table: "District",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Ilha de SÃ£o Jorge");
        }
    }
}
