using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class seed_location_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Portugal" });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Aveiro" },
                    { 2, 1, "Beja" },
                    { 3, 1, "Braga" },
                    { 4, 1, "Bragança" },
                    { 5, 1, "Castelo Branco" },
                    { 6, 1, "Coimbra" },
                    { 7, 1, "Évora" },
                    { 8, 1, "Faro" },
                    { 9, 1, "Guarda" },
                    { 10, 1, "Leiria" },
                    { 11, 1, "Lisboa" },
                    { 12, 1, "Portalegre" },
                    { 13, 1, "Porto" },
                    { 14, 1, "Santarém" },
                    { 15, 1, "Setúbal" },
                    { 16, 1, "Viana do Castelo" },
                    { 17, 1, "Vila Real" },
                    { 18, 1, "Viseu" },
                    { 19, 1, "Ilha da Madeira" },
                    { 20, 1, "Ilha de Porto Santo" },
                    { 21, 1, "Ilha de Santa Maria" },
                    { 22, 1, "Ilha de São Miguel" },
                    { 23, 1, "Ilha Terceira" },
                    { 24, 1, "Ilha Graciosa" },
                    { 25, 1, "Ilha de São Jorge" },
                    { 26, 1, "Ilha do Pico" },
                    { 27, 1, "Ilha do Faial" },
                    { 28, 1, "Ilha das Flores" },
                    { 29, 1, "Ilha do Corvo" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Águeda" },
                    { 2, 1, "Albergaria-a-Velha" },
                    { 3, 1, "Anadia" },
                    { 4, 1, "Arouca" },
                    { 5, 1, "Aveiro" },
                    { 6, 1, "Castelo de Paiva" },
                    { 7, 1, "Espinho" },
                    { 8, 1, "Estarreja" },
                    { 9, 1, "Santa Maria da Feira" },
                    { 10, 1, "Ílhavo" },
                    { 11, 1, "Mealhada" },
                    { 12, 1, "Murtosa" },
                    { 13, 1, "Oliveira de Azeméis" },
                    { 14, 1, "Oliveira do Bairro" },
                    { 15, 1, "Ovar" },
                    { 16, 1, "São João da Madeira" },
                    { 17, 1, "Sever do Vouga" },
                    { 18, 1, "Vagos" },
                    { 19, 1, "Vale de Cambra" },
                    { 20, 2, "Aljustrel" },
                    { 21, 2, "Almodôvar" },
                    { 22, 2, "Alvito" },
                    { 23, 2, "Barrancos" },
                    { 24, 2, "Beja" },
                    { 25, 2, "Castro Verde" },
                    { 26, 2, "Cuba" },
                    { 27, 2, "Ferreira do Alentejo" },
                    { 28, 2, "Mértola" },
                    { 29, 2, "Moura" },
                    { 30, 2, "Odemira" },
                    { 31, 2, "Ourique" },
                    { 32, 2, "Serpa" },
                    { 33, 2, "Vidigueira" },
                    { 34, 3, "Amares" },
                    { 35, 3, "Barcelos" },
                    { 36, 3, "Braga" },
                    { 37, 3, "Cabeceiras de Basto" },
                    { 38, 3, "Celorico de Basto" },
                    { 39, 3, "Esposende" },
                    { 40, 3, "Fafe" },
                    { 41, 3, "Guimarães" },
                    { 42, 3, "Póvoa de Lanhoso" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 43, 3, "Terras de Bouro" },
                    { 44, 3, "Vieira do Minho" },
                    { 45, 3, "Vila Nova de Famalicão" },
                    { 46, 3, "Vila Verde" },
                    { 47, 3, "Vizela" },
                    { 48, 4, "Alfândega da Fé" },
                    { 49, 4, "Bragança" },
                    { 50, 4, "Carrazeda de Ansiães" },
                    { 51, 4, "Freixo de Espada à Cinta" },
                    { 52, 4, "Macedo de Cavaleiros" },
                    { 53, 4, "Miranda do Douro" },
                    { 54, 4, "Mirandela" },
                    { 55, 4, "Mogadouro" },
                    { 56, 4, "Torre de Moncorvo" },
                    { 57, 4, "Vila Flor" },
                    { 58, 4, "Vimioso" },
                    { 59, 4, "Vinhais" },
                    { 60, 5, "Belmonte" },
                    { 61, 5, "Castelo Branco" },
                    { 62, 5, "Covilhã" },
                    { 63, 5, "Fundão" },
                    { 64, 5, "Idanha-a-Nova" },
                    { 65, 5, "Oleiros" },
                    { 66, 5, "Penamacor" },
                    { 67, 5, "Proença-a-Nova" },
                    { 68, 5, "Sertã" },
                    { 69, 5, "Vila de Rei" },
                    { 70, 5, "Vila Velha de Ródão" },
                    { 71, 6, "Arganil" },
                    { 72, 6, "Cantanhede" },
                    { 73, 6, "Coimbra" },
                    { 74, 6, "Condeixa-a-Nova" },
                    { 75, 6, "Figueira da Foz" },
                    { 76, 6, "Góis" },
                    { 77, 6, "Lousã" },
                    { 78, 6, "Mira" },
                    { 79, 6, "Miranda do Corvo" },
                    { 80, 6, "Montemor-o-Velho" },
                    { 81, 6, "Oliveira do Hospital" },
                    { 82, 6, "Pampilhosa da Serra" },
                    { 83, 6, "Penacova" },
                    { 84, 6, "Penela" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 85, 6, "Soure" },
                    { 86, 6, "Tábua" },
                    { 87, 6, "Vila Nova de Poiares" },
                    { 88, 7, "Alandroal" },
                    { 89, 7, "Arraiolos" },
                    { 90, 7, "Borba" },
                    { 91, 7, "Estremoz" },
                    { 92, 7, "Évora" },
                    { 93, 7, "Montemor-o-Novo" },
                    { 94, 7, "Mora" },
                    { 95, 7, "Mourão" },
                    { 96, 7, "Portel" },
                    { 97, 7, "Redondo" },
                    { 98, 7, "Reguengos de Monsaraz" },
                    { 99, 7, "Vendas Novas" },
                    { 100, 7, "Viana do Alentejo" },
                    { 101, 7, "Vila Viçosa" },
                    { 102, 8, "Albufeira" },
                    { 103, 8, "Alcoutim" },
                    { 104, 8, "Aljezur" },
                    { 105, 8, "Castro Marim" },
                    { 106, 8, "Faro" },
                    { 107, 8, "Lagoa" },
                    { 108, 8, "Lagos" },
                    { 109, 8, "Loulé" },
                    { 110, 8, "Monchique" },
                    { 111, 8, "Olhão" },
                    { 112, 8, "Portimão" },
                    { 113, 8, "São Brás de Alportel" },
                    { 114, 8, "Silves" },
                    { 115, 8, "Tavira" },
                    { 116, 8, "Vila do Bispo" },
                    { 117, 8, "Vila Real de Santo António" },
                    { 118, 9, "Aguiar da Beira" },
                    { 119, 9, "Almeida" },
                    { 120, 9, "Celorico da Beira" },
                    { 121, 9, "Figueira de Castelo Rodrigo" },
                    { 122, 9, "Fornos de Algodres" },
                    { 123, 9, "Gouveia" },
                    { 124, 9, "Guarda" },
                    { 125, 9, "Manteigas" },
                    { 126, 9, "Mêda" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 127, 9, "Pinhel" },
                    { 128, 9, "Sabugal" },
                    { 129, 9, "Seia" },
                    { 130, 9, "Trancoso" },
                    { 131, 9, "Vila Nova de Foz Côa" },
                    { 132, 10, "Alcobaça" },
                    { 133, 10, "Alvaiázere" },
                    { 134, 10, "Ansião" },
                    { 135, 10, "Batalha" },
                    { 136, 10, "Bombarral" },
                    { 137, 10, "Caldas da Rainha" },
                    { 138, 10, "Castanheira de Pêra" },
                    { 139, 10, "Figueiró dos Vinhos" },
                    { 140, 10, "Leiria" },
                    { 141, 10, "Marinha Grande" },
                    { 142, 10, "Nazaré" },
                    { 143, 10, "Óbidos" },
                    { 144, 10, "Pedrógão Grande" },
                    { 145, 10, "Peniche" },
                    { 146, 10, "Pombal" },
                    { 147, 10, "Porto de Mós" },
                    { 148, 11, "Alenquer" },
                    { 149, 11, "Arruda dos Vinhos" },
                    { 150, 11, "Azambuja" },
                    { 151, 11, "Cadaval" },
                    { 152, 11, "Cascais" },
                    { 153, 11, "Lisboa" },
                    { 154, 11, "Loures" },
                    { 155, 11, "Lourinhã" },
                    { 156, 11, "Mafra" },
                    { 157, 11, "Oeiras" },
                    { 158, 11, "Sintra" },
                    { 159, 11, "Sobral de Monte Agraço" },
                    { 160, 11, "Torres Vedras" },
                    { 161, 11, "Vila Franca de Xira" },
                    { 162, 11, "Amadora" },
                    { 163, 11, "Odivelas" },
                    { 164, 12, "Alter do Chão" },
                    { 165, 12, "Arronches" },
                    { 166, 12, "Avis" },
                    { 167, 12, "Campo Maior" },
                    { 168, 12, "Castelo de Vide" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 169, 12, "Crato" },
                    { 170, 12, "Elvas" },
                    { 171, 12, "Fronteira" },
                    { 172, 12, "Gavião" },
                    { 173, 12, "Marvão" },
                    { 174, 12, "Monforte" },
                    { 175, 12, "Nisa" },
                    { 176, 12, "Ponte de Sor" },
                    { 177, 12, "Portalegre" },
                    { 178, 12, "Sousel" },
                    { 179, 13, "Amarante" },
                    { 180, 13, "Baião" },
                    { 181, 13, "Felgueiras" },
                    { 182, 13, "Gondomar" },
                    { 183, 13, "Lousada" },
                    { 184, 13, "Maia" },
                    { 185, 13, "Marco de Canaveses" },
                    { 186, 13, "Matosinhos" },
                    { 187, 13, "Paços de Ferreira" },
                    { 188, 13, "Paredes" },
                    { 189, 13, "Penafiel" },
                    { 190, 13, "Porto" },
                    { 191, 13, "Póvoa de Varzim" },
                    { 192, 13, "Santo Tirso" },
                    { 193, 13, "Valongo" },
                    { 194, 13, "Vila do Conde" },
                    { 195, 13, "Vila Nova de Gaia" },
                    { 196, 13, "Trofa" },
                    { 197, 14, "Abrantes" },
                    { 198, 14, "Alcanena" },
                    { 199, 14, "Almeirim" },
                    { 200, 14, "Alpiarça" },
                    { 201, 14, "Benavente" },
                    { 202, 14, "Cartaxo" },
                    { 203, 14, "Chamusca" },
                    { 204, 14, "Constância" },
                    { 205, 14, "Coruche" },
                    { 206, 14, "Entroncamento" },
                    { 207, 14, "Ferreira do Zêzere" },
                    { 208, 14, "Golegã" },
                    { 209, 14, "Mação" },
                    { 210, 14, "Rio Maior" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 211, 14, "Salvaterra de Magos" },
                    { 212, 14, "Santarém" },
                    { 213, 14, "Sardoal" },
                    { 214, 14, "Tomar" },
                    { 215, 14, "Torres Novas" },
                    { 216, 14, "Vila Nova da Barquinha" },
                    { 217, 14, "Ourém" },
                    { 218, 15, "Alcácer do Sal" },
                    { 219, 15, "Alcochete" },
                    { 220, 15, "Almada" },
                    { 221, 15, "Barreiro" },
                    { 222, 15, "Grândola" },
                    { 223, 15, "Moita" },
                    { 224, 15, "Montijo" },
                    { 225, 15, "Palmela" },
                    { 226, 15, "Santiago do Cacém" },
                    { 227, 15, "Seixal" },
                    { 228, 15, "Sesimbra" },
                    { 229, 15, "Setúbal" },
                    { 230, 15, "Sines" },
                    { 231, 16, "Arcos de Valdevez" },
                    { 232, 16, "Caminha" },
                    { 233, 16, "Melgaço" },
                    { 234, 16, "Monção" },
                    { 235, 16, "Paredes de Coura" },
                    { 236, 16, "Ponte da Barca" },
                    { 237, 16, "Ponte de Lima" },
                    { 238, 16, "Valença" },
                    { 239, 16, "Viana do Castelo" },
                    { 240, 16, "Vila Nova de Cerveira" },
                    { 241, 17, "Alijó" },
                    { 242, 17, "Boticas" },
                    { 243, 17, "Chaves" },
                    { 244, 17, "Mesão Frio" },
                    { 245, 17, "Mondim de Basto" },
                    { 246, 17, "Montalegre" },
                    { 247, 17, "Murça" },
                    { 248, 17, "Peso da Régua" },
                    { 249, 17, "Ribeira de Pena" },
                    { 250, 17, "Sabrosa" },
                    { 251, 17, "Santa Marta de Penaguião" },
                    { 252, 17, "Valpaços" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 253, 17, "Vila Pouca de Aguiar" },
                    { 254, 17, "Vila Real" },
                    { 255, 18, "Armamar" },
                    { 256, 18, "Carregal do Sal" },
                    { 257, 18, "Castro Daire" },
                    { 258, 18, "Cinfães" },
                    { 259, 18, "Lamego" },
                    { 260, 18, "Mangualde" },
                    { 261, 18, "Moimenta da Beira" },
                    { 262, 18, "Mortágua" },
                    { 263, 18, "Nelas" },
                    { 264, 18, "Oliveira de Frades" },
                    { 265, 18, "Penalva do Castelo" },
                    { 266, 18, "Penedono" },
                    { 267, 18, "Resende" },
                    { 268, 18, "Santa Comba Dão" },
                    { 269, 18, "São João da Pesqueira" },
                    { 270, 18, "São Pedro do Sul" },
                    { 271, 18, "Sátão" },
                    { 272, 18, "Sernancelhe" },
                    { 273, 18, "Tabuaço" },
                    { 274, 18, "Tarouca" },
                    { 275, 18, "Tondela" },
                    { 276, 18, "Vila Nova de Paiva" },
                    { 277, 18, "Viseu" },
                    { 278, 18, "Vouzela" },
                    { 279, 19, "Calheta" },
                    { 280, 19, "Câmara de Lobos" },
                    { 281, 19, "Funchal" },
                    { 282, 19, "Machico" },
                    { 283, 19, "Ponta do Sol" },
                    { 284, 19, "Porto Moniz" },
                    { 285, 19, "Ribeira Brava" },
                    { 286, 19, "Santa Cruz" },
                    { 287, 19, "Santana" },
                    { 288, 19, "São Vicente" },
                    { 289, 20, "Porto Santo" },
                    { 290, 21, "Vila do Porto" },
                    { 291, 22, "Lagoa" },
                    { 292, 22, "Nordeste" },
                    { 293, 22, "Ponta Delgada" },
                    { 294, 22, "Povoação" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name" },
                values: new object[,]
                {
                    { 295, 22, "Ribeira Grande" },
                    { 296, 22, "Vila Franca do Campo" },
                    { 297, 23, "Angra do Heroísmo" },
                    { 298, 23, "Vila da Praia da Vitória" },
                    { 299, 24, "Santa Cruz da Graciosa" },
                    { 300, 25, "Calheta" },
                    { 301, 25, "Velas" },
                    { 302, 26, "Lajes do Pico" },
                    { 303, 26, "Madalena" },
                    { 304, 26, "São Roque do Pico" },
                    { 305, 27, "Horta" },
                    { 306, 28, "Lajes das Flores" },
                    { 307, 28, "Santa Cruz das Flores" },
                    { 308, 29, "Corvo" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 184);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 225);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 226);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 227);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 228);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 229);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 236);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 237);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 238);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 239);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 240);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 241);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 242);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 243);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 244);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 245);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 246);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 247);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 248);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 249);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 250);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 251);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 252);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 253);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 254);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 255);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 256);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 257);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 258);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 259);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 260);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 261);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 262);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 263);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 264);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 265);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 266);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 267);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 268);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 269);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 270);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 271);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 272);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 273);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 274);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 275);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 276);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 277);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 278);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 279);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 280);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 281);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 282);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 283);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 284);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 285);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 286);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 287);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 288);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 289);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 290);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 291);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 292);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 293);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 294);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 295);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 296);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 297);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 298);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 299);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 302);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 303);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 304);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 305);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 307);

            migrationBuilder.DeleteData(
                table: "County",
                keyColumn: "Id",
                keyValue: 308);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
