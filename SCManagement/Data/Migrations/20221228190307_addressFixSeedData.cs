using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class addressFixSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "District",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "County",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 1, "Portugal", "portugal" });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "CountryId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, 1, "Aveiro", "aveiro" },
                    { 2, 1, "Beja", "beja" },
                    { 3, 1, "Braga", "braga" },
                    { 4, 1, "Bragança", "braganca" },
                    { 5, 1, "Castelo Branco", "castelo branco" },
                    { 6, 1, "Coimbra", "coimbra" },
                    { 7, 1, "Évora", "evora" },
                    { 8, 1, "Faro", "faro" },
                    { 9, 1, "Guarda", "guarda" },
                    { 10, 1, "Leiria", "leiria" },
                    { 11, 1, "Lisboa", "lisboa" },
                    { 12, 1, "Portalegre", "portalegre" },
                    { 13, 1, "Porto", "porto" },
                    { 14, 1, "Santarém", "santarem" },
                    { 15, 1, "Setúbal", "setubal" },
                    { 16, 1, "Viana do Castelo", "viana do castelo" },
                    { 17, 1, "Vila Real", "vila real" },
                    { 18, 1, "Viseu", "viseu" },
                    { 19, 1, "Ilha da Madeira", "ilha da madeira" },
                    { 20, 1, "Ilha de Porto Santo", "ilha de porto santo" },
                    { 21, 1, "Ilha de Santa Maria", "ilha de santa maria" },
                    { 22, 1, "Ilha de São Miguel", "ilha de sao miguel" },
                    { 23, 1, "Ilha Terceira", "ilha terceira" },
                    { 24, 1, "Ilha Graciosa", "ilha graciosa" },
                    { 25, 1, "Ilha de São Jorge", "ilha de sao jorge" },
                    { 26, 1, "Ilha do Pico", "ilha do pico" },
                    { 27, 1, "Ilha do Faial", "ilha do faial" },
                    { 28, 1, "Ilha das Flores", "ilha das flores" },
                    { 29, 1, "Ilha do Corvo", "ilha do corvo" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, 1, "Águeda", "agueda" },
                    { 2, 1, "Albergaria-a-Velha", "albergaria-a-velha" },
                    { 3, 1, "Anadia", "anadia" },
                    { 4, 1, "Arouca", "arouca" },
                    { 5, 1, "Aveiro", "aveiro" },
                    { 6, 1, "Castelo de Paiva", "castelo de paiva" },
                    { 7, 1, "Espinho", "espinho" },
                    { 8, 1, "Estarreja", "estarreja" },
                    { 9, 1, "Santa Maria da Feira", "santa maria da feira" },
                    { 10, 1, "Ílhavo", "ilhavo" },
                    { 11, 1, "Mealhada", "mealhada" },
                    { 12, 1, "Murtosa", "murtosa" },
                    { 13, 1, "Oliveira de Azeméis", "oliveira de azemeis" },
                    { 14, 1, "Oliveira do Bairro", "oliveira do bairro" },
                    { 15, 1, "Ovar", "ovar" },
                    { 16, 1, "São João da Madeira", "sao joao da madeira" },
                    { 17, 1, "Sever do Vouga", "sever do vouga" },
                    { 18, 1, "Vagos", "vagos" },
                    { 19, 1, "Vale de Cambra", "vale de cambra" },
                    { 20, 2, "Aljustrel", "aljustrel" },
                    { 21, 2, "Almodôvar", "almodovar" },
                    { 22, 2, "Alvito", "alvito" },
                    { 23, 2, "Barrancos", "barrancos" },
                    { 24, 2, "Beja", "beja" },
                    { 25, 2, "Castro Verde", "castro verde" },
                    { 26, 2, "Cuba", "cuba" },
                    { 27, 2, "Ferreira do Alentejo", "ferreira do alentejo" },
                    { 28, 2, "Mértola", "mertola" },
                    { 29, 2, "Moura", "moura" },
                    { 30, 2, "Odemira", "odemira" },
                    { 31, 2, "Ourique", "ourique" },
                    { 32, 2, "Serpa", "serpa" },
                    { 33, 2, "Vidigueira", "vidigueira" },
                    { 34, 3, "Amares", "amares" },
                    { 35, 3, "Barcelos", "barcelos" },
                    { 36, 3, "Braga", "braga" },
                    { 37, 3, "Cabeceiras de Basto", "cabeceiras de basto" },
                    { 38, 3, "Celorico de Basto", "celorico de basto" },
                    { 39, 3, "Esposende", "esposende" },
                    { 40, 3, "Fafe", "fafe" },
                    { 41, 3, "Guimarães", "guimaraes" },
                    { 42, 3, "Póvoa de Lanhoso", "povoa de lanhoso" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 43, 3, "Terras de Bouro", "terras de bouro" },
                    { 44, 3, "Vieira do Minho", "vieira do minho" },
                    { 45, 3, "Vila Nova de Famalicão", "vila nova de famalicao" },
                    { 46, 3, "Vila Verde", "vila verde" },
                    { 47, 3, "Vizela", "vizela" },
                    { 48, 4, "Alfândega da Fé", "alfandega da fe" },
                    { 49, 4, "Bragança", "braganca" },
                    { 50, 4, "Carrazeda de Ansiães", "carrazeda de ansiaes" },
                    { 51, 4, "Freixo de Espada à Cinta", "freixo de espada a cinta" },
                    { 52, 4, "Macedo de Cavaleiros", "macedo de cavaleiros" },
                    { 53, 4, "Miranda do Douro", "miranda do douro" },
                    { 54, 4, "Mirandela", "mirandela" },
                    { 55, 4, "Mogadouro", "mogadouro" },
                    { 56, 4, "Torre de Moncorvo", "torre de moncorvo" },
                    { 57, 4, "Vila Flor", "vila flor" },
                    { 58, 4, "Vimioso", "vimioso" },
                    { 59, 4, "Vinhais", "vinhais" },
                    { 60, 5, "Belmonte", "belmonte" },
                    { 61, 5, "Castelo Branco", "castelo branco" },
                    { 62, 5, "Covilhã", "covilha" },
                    { 63, 5, "Fundão", "fundao" },
                    { 64, 5, "Idanha-a-Nova", "idanha-a-nova" },
                    { 65, 5, "Oleiros", "oleiros" },
                    { 66, 5, "Penamacor", "penamacor" },
                    { 67, 5, "Proença-a-Nova", "proenca-a-nova" },
                    { 68, 5, "Sertã", "serta" },
                    { 69, 5, "Vila de Rei", "vila de rei" },
                    { 70, 5, "Vila Velha de Ródão", "vila velha de rodao" },
                    { 71, 6, "Arganil", "arganil" },
                    { 72, 6, "Cantanhede", "cantanhede" },
                    { 73, 6, "Coimbra", "coimbra" },
                    { 74, 6, "Condeixa-a-Nova", "condeixa-a-nova" },
                    { 75, 6, "Figueira da Foz", "figueira da foz" },
                    { 76, 6, "Góis", "gois" },
                    { 77, 6, "Lousã", "lousa" },
                    { 78, 6, "Mira", "mira" },
                    { 79, 6, "Miranda do Corvo", "miranda do corvo" },
                    { 80, 6, "Montemor-o-Velho", "montemor-o-velho" },
                    { 81, 6, "Oliveira do Hospital", "oliveira do hospital" },
                    { 82, 6, "Pampilhosa da Serra", "pampilhosa da serra" },
                    { 83, 6, "Penacova", "penacova" },
                    { 84, 6, "Penela", "penela" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 85, 6, "Soure", "soure" },
                    { 86, 6, "Tábua", "tabua" },
                    { 87, 6, "Vila Nova de Poiares", "vila nova de poiares" },
                    { 88, 7, "Alandroal", "alandroal" },
                    { 89, 7, "Arraiolos", "arraiolos" },
                    { 90, 7, "Borba", "borba" },
                    { 91, 7, "Estremoz", "estremoz" },
                    { 92, 7, "Évora", "evora" },
                    { 93, 7, "Montemor-o-Novo", "montemor-o-novo" },
                    { 94, 7, "Mora", "mora" },
                    { 95, 7, "Mourão", "mourao" },
                    { 96, 7, "Portel", "portel" },
                    { 97, 7, "Redondo", "redondo" },
                    { 98, 7, "Reguengos de Monsaraz", "reguengos de monsaraz" },
                    { 99, 7, "Vendas Novas", "vendas novas" },
                    { 100, 7, "Viana do Alentejo", "viana do alentejo" },
                    { 101, 7, "Vila Viçosa", "vila vicosa" },
                    { 102, 8, "Albufeira", "albufeira" },
                    { 103, 8, "Alcoutim", "alcoutim" },
                    { 104, 8, "Aljezur", "aljezur" },
                    { 105, 8, "Castro Marim", "castro marim" },
                    { 106, 8, "Faro", "faro" },
                    { 107, 8, "Lagoa", "lagoa" },
                    { 108, 8, "Lagos", "lagos" },
                    { 109, 8, "Loulé", "loule" },
                    { 110, 8, "Monchique", "monchique" },
                    { 111, 8, "Olhão", "olhao" },
                    { 112, 8, "Portimão", "portimao" },
                    { 113, 8, "São Brás de Alportel", "sao bras de alportel" },
                    { 114, 8, "Silves", "silves" },
                    { 115, 8, "Tavira", "tavira" },
                    { 116, 8, "Vila do Bispo", "vila do bispo" },
                    { 117, 8, "Vila Real de Santo António", "vila real de santo antonio" },
                    { 118, 9, "Aguiar da Beira", "aguiar da beira" },
                    { 119, 9, "Almeida", "almeida" },
                    { 120, 9, "Celorico da Beira", "celorico da beira" },
                    { 121, 9, "Figueira de Castelo Rodrigo", "figueira de castelo rodrigo" },
                    { 122, 9, "Fornos de Algodres", "fornos de algodres" },
                    { 123, 9, "Gouveia", "gouveia" },
                    { 124, 9, "Guarda", "guarda" },
                    { 125, 9, "Manteigas", "manteigas" },
                    { 126, 9, "Mêda", "meda" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 127, 9, "Pinhel", "pinhel" },
                    { 128, 9, "Sabugal", "sabugal" },
                    { 129, 9, "Seia", "seia" },
                    { 130, 9, "Trancoso", "trancoso" },
                    { 131, 9, "Vila Nova de Foz Côa", "vila nova de foz coa" },
                    { 132, 10, "Alcobaça", "alcobaca" },
                    { 133, 10, "Alvaiázere", "alvaiazere" },
                    { 134, 10, "Ansião", "ansiao" },
                    { 135, 10, "Batalha", "batalha" },
                    { 136, 10, "Bombarral", "bombarral" },
                    { 137, 10, "Caldas da Rainha", "caldas da rainha" },
                    { 138, 10, "Castanheira de Pêra", "castanheira de pera" },
                    { 139, 10, "Figueiró dos Vinhos", "figueiro dos vinhos" },
                    { 140, 10, "Leiria", "leiria" },
                    { 141, 10, "Marinha Grande", "marinha grande" },
                    { 142, 10, "Nazaré", "nazare" },
                    { 143, 10, "Óbidos", "obidos" },
                    { 144, 10, "Pedrógão Grande", "pedrogao grande" },
                    { 145, 10, "Peniche", "peniche" },
                    { 146, 10, "Pombal", "pombal" },
                    { 147, 10, "Porto de Mós", "porto de mos" },
                    { 148, 11, "Alenquer", "alenquer" },
                    { 149, 11, "Arruda dos Vinhos", "arruda dos vinhos" },
                    { 150, 11, "Azambuja", "azambuja" },
                    { 151, 11, "Cadaval", "cadaval" },
                    { 152, 11, "Cascais", "cascais" },
                    { 153, 11, "Lisboa", "lisboa" },
                    { 154, 11, "Loures", "loures" },
                    { 155, 11, "Lourinhã", "lourinha" },
                    { 156, 11, "Mafra", "mafra" },
                    { 157, 11, "Oeiras", "oeiras" },
                    { 158, 11, "Sintra", "sintra" },
                    { 159, 11, "Sobral de Monte Agraço", "sobral de monte agraco" },
                    { 160, 11, "Torres Vedras", "torres vedras" },
                    { 161, 11, "Vila Franca de Xira", "vila franca de xira" },
                    { 162, 11, "Amadora", "amadora" },
                    { 163, 11, "Odivelas", "odivelas" },
                    { 164, 12, "Alter do Chão", "alter do chao" },
                    { 165, 12, "Arronches", "arronches" },
                    { 166, 12, "Avis", "avis" },
                    { 167, 12, "Campo Maior", "campo maior" },
                    { 168, 12, "Castelo de Vide", "castelo de vide" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 169, 12, "Crato", "crato" },
                    { 170, 12, "Elvas", "elvas" },
                    { 171, 12, "Fronteira", "fronteira" },
                    { 172, 12, "Gavião", "gaviao" },
                    { 173, 12, "Marvão", "marvao" },
                    { 174, 12, "Monforte", "monforte" },
                    { 175, 12, "Nisa", "nisa" },
                    { 176, 12, "Ponte de Sor", "ponte de sor" },
                    { 177, 12, "Portalegre", "portalegre" },
                    { 178, 12, "Sousel", "sousel" },
                    { 179, 13, "Amarante", "amarante" },
                    { 180, 13, "Baião", "baiao" },
                    { 181, 13, "Felgueiras", "felgueiras" },
                    { 182, 13, "Gondomar", "gondomar" },
                    { 183, 13, "Lousada", "lousada" },
                    { 184, 13, "Maia", "maia" },
                    { 185, 13, "Marco de Canaveses", "marco de canaveses" },
                    { 186, 13, "Matosinhos", "matosinhos" },
                    { 187, 13, "Paços de Ferreira", "pacos de ferreira" },
                    { 188, 13, "Paredes", "paredes" },
                    { 189, 13, "Penafiel", "penafiel" },
                    { 190, 13, "Porto", "porto" },
                    { 191, 13, "Póvoa de Varzim", "povoa de varzim" },
                    { 192, 13, "Santo Tirso", "santo tirso" },
                    { 193, 13, "Valongo", "valongo" },
                    { 194, 13, "Vila do Conde", "vila do conde" },
                    { 195, 13, "Vila Nova de Gaia", "vila nova de gaia" },
                    { 196, 13, "Trofa", "trofa" },
                    { 197, 14, "Abrantes", "abrantes" },
                    { 198, 14, "Alcanena", "alcanena" },
                    { 199, 14, "Almeirim", "almeirim" },
                    { 200, 14, "Alpiarça", "alpiarca" },
                    { 201, 14, "Benavente", "benavente" },
                    { 202, 14, "Cartaxo", "cartaxo" },
                    { 203, 14, "Chamusca", "chamusca" },
                    { 204, 14, "Constância", "constancia" },
                    { 205, 14, "Coruche", "coruche" },
                    { 206, 14, "Entroncamento", "entroncamento" },
                    { 207, 14, "Ferreira do Zêzere", "ferreira do zezere" },
                    { 208, 14, "Golegã", "golega" },
                    { 209, 14, "Mação", "macao" },
                    { 210, 14, "Rio Maior", "rio maior" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 211, 14, "Salvaterra de Magos", "salvaterra de magos" },
                    { 212, 14, "Santarém", "santarem" },
                    { 213, 14, "Sardoal", "sardoal" },
                    { 214, 14, "Tomar", "tomar" },
                    { 215, 14, "Torres Novas", "torres novas" },
                    { 216, 14, "Vila Nova da Barquinha", "vila nova da barquinha" },
                    { 217, 14, "Ourém", "ourem" },
                    { 218, 15, "Alcácer do Sal", "alcacer do sal" },
                    { 219, 15, "Alcochete", "alcochete" },
                    { 220, 15, "Almada", "almada" },
                    { 221, 15, "Barreiro", "barreiro" },
                    { 222, 15, "Grândola", "grandola" },
                    { 223, 15, "Moita", "moita" },
                    { 224, 15, "Montijo", "montijo" },
                    { 225, 15, "Palmela", "palmela" },
                    { 226, 15, "Santiago do Cacém", "santiago do cacem" },
                    { 227, 15, "Seixal", "seixal" },
                    { 228, 15, "Sesimbra", "sesimbra" },
                    { 229, 15, "Setúbal", "setubal" },
                    { 230, 15, "Sines", "sines" },
                    { 231, 16, "Arcos de Valdevez", "arcos de valdevez" },
                    { 232, 16, "Caminha", "caminha" },
                    { 233, 16, "Melgaço", "melgaco" },
                    { 234, 16, "Monção", "moncao" },
                    { 235, 16, "Paredes de Coura", "paredes de coura" },
                    { 236, 16, "Ponte da Barca", "ponte da barca" },
                    { 237, 16, "Ponte de Lima", "ponte de lima" },
                    { 238, 16, "Valença", "valenca" },
                    { 239, 16, "Viana do Castelo", "viana do castelo" },
                    { 240, 16, "Vila Nova de Cerveira", "vila nova de cerveira" },
                    { 241, 17, "Alijó", "alijo" },
                    { 242, 17, "Boticas", "boticas" },
                    { 243, 17, "Chaves", "chaves" },
                    { 244, 17, "Mesão Frio", "mesao frio" },
                    { 245, 17, "Mondim de Basto", "mondim de basto" },
                    { 246, 17, "Montalegre", "montalegre" },
                    { 247, 17, "Murça", "murca" },
                    { 248, 17, "Peso da Régua", "peso da regua" },
                    { 249, 17, "Ribeira de Pena", "ribeira de pena" },
                    { 250, 17, "Sabrosa", "sabrosa" },
                    { 251, 17, "Santa Marta de Penaguião", "santa marta de penaguiao" },
                    { 252, 17, "Valpaços", "valpacos" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 253, 17, "Vila Pouca de Aguiar", "vila pouca de aguiar" },
                    { 254, 17, "Vila Real", "vila real" },
                    { 255, 18, "Armamar", "armamar" },
                    { 256, 18, "Carregal do Sal", "carregal do sal" },
                    { 257, 18, "Castro Daire", "castro daire" },
                    { 258, 18, "Cinfães", "cinfaes" },
                    { 259, 18, "Lamego", "lamego" },
                    { 260, 18, "Mangualde", "mangualde" },
                    { 261, 18, "Moimenta da Beira", "moimenta da beira" },
                    { 262, 18, "Mortágua", "mortagua" },
                    { 263, 18, "Nelas", "nelas" },
                    { 264, 18, "Oliveira de Frades", "oliveira de frades" },
                    { 265, 18, "Penalva do Castelo", "penalva do castelo" },
                    { 266, 18, "Penedono", "penedono" },
                    { 267, 18, "Resende", "resende" },
                    { 268, 18, "Santa Comba Dão", "santa comba dao" },
                    { 269, 18, "São João da Pesqueira", "sao joao da pesqueira" },
                    { 270, 18, "São Pedro do Sul", "sao pedro do sul" },
                    { 271, 18, "Sátão", "satao" },
                    { 272, 18, "Sernancelhe", "sernancelhe" },
                    { 273, 18, "Tabuaço", "tabuaco" },
                    { 274, 18, "Tarouca", "tarouca" },
                    { 275, 18, "Tondela", "tondela" },
                    { 276, 18, "Vila Nova de Paiva", "vila nova de paiva" },
                    { 277, 18, "Viseu", "viseu" },
                    { 278, 18, "Vouzela", "vouzela" },
                    { 279, 19, "Calheta", "calheta" },
                    { 280, 19, "Câmara de Lobos", "camara de lobos" },
                    { 281, 19, "Funchal", "funchal" },
                    { 282, 19, "Machico", "machico" },
                    { 283, 19, "Ponta do Sol", "ponta do sol" },
                    { 284, 19, "Porto Moniz", "porto moniz" },
                    { 285, 19, "Ribeira Brava", "ribeira brava" },
                    { 286, 19, "Santa Cruz", "santa cruz" },
                    { 287, 19, "Santana", "santana" },
                    { 288, 19, "São Vicente", "sao vicente" },
                    { 289, 20, "Porto Santo", "porto santo" },
                    { 290, 21, "Vila do Porto", "vila do porto" },
                    { 291, 22, "Lagoa", "lagoa" },
                    { 292, 22, "Nordeste", "nordeste" },
                    { 293, 22, "Ponta Delgada", "ponta delgada" },
                    { 294, 22, "Povoação", "povoacao" }
                });

            migrationBuilder.InsertData(
                table: "County",
                columns: new[] { "Id", "DistrictId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 295, 22, "Ribeira Grande", "ribeira grande" },
                    { 296, 22, "Vila Franca do Campo", "vila franca do campo" },
                    { 297, 23, "Angra do Heroísmo", "angra do heroismo" },
                    { 298, 23, "Vila da Praia da Vitória", "vila da praia da vitoria" },
                    { 299, 24, "Santa Cruz da Graciosa", "santa cruz da graciosa" },
                    { 300, 25, "Calheta", "calheta" },
                    { 301, 25, "Velas", "velas" },
                    { 302, 26, "Lajes do Pico", "lajes do pico" },
                    { 303, 26, "Madalena", "madalena" },
                    { 304, 26, "São Roque do Pico", "sao roque do pico" },
                    { 305, 27, "Horta", "horta" },
                    { 306, 28, "Lajes das Flores", "lajes das flores" },
                    { 307, 28, "Santa Cruz das Flores", "santa cruz das flores" },
                    { 308, 29, "Corvo", "corvo" }
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

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "District");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "County");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Country");
        }
    }
}
