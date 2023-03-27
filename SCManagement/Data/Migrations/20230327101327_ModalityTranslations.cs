using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class ModalityTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Modality");

            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "TrainingPlanSession",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "TrainingPlanSession",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ModalityTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModalityId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Atribute = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalityTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModalityTranslation_Modality_ModalityId",
                        column: x => x.ModalityId,
                        principalTable: "Modality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ModalityTranslation",
                columns: new[] { "Id", "Atribute", "Language", "ModalityId", "Value" },
                values: new object[,]
                {
                    { 1, "Name", "pt-PT", 1, "Atletismo" },
                    { 2, "Name", "en-US", 1, "Athletics" },
                    { 3, "Name", "pt-PT", 2, "Basquetebol" },
                    { 4, "Name", "en-US", 2, "Basketball" },
                    { 5, "Name", "pt-PT", 3, "Futebol" },
                    { 6, "Name", "en-US", 3, "Football" },
                    { 7, "Name", "pt-PT", 4, "Futsal" },
                    { 8, "Name", "en-US", 4, "Futsal" },
                    { 9, "Name", "pt-PT", 5, "Hóquei em Patins" },
                    { 10, "Name", "en-US", 5, "Roller Hockey" },
                    { 11, "Name", "pt-PT", 6, "Natação" },
                    { 12, "Name", "en-US", 6, "Swimming" },
                    { 13, "Name", "pt-PT", 7, "Voleibol" },
                    { 14, "Name", "en-US", 7, "Volleyball" },
                    { 15, "Name", "pt-PT", 8, "BTT" },
                    { 16, "Name", "en-US", 8, "BTT" },
                    { 17, "Name", "pt-PT", 9, "Taekwondo" },
                    { 18, "Name", "en-US", 9, "Taekwondo" },
                    { 19, "Name", "pt-PT", 10, "Orientação" },
                    { 20, "Name", "en-US", 10, "Orientation" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModalityTranslation_ModalityId",
                table: "ModalityTranslation",
                column: "ModalityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModalityTranslation");

            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "TrainingPlanSession",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "TrainingPlanSession",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Modality",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Atletismo");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Basquetebol");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Futebol");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Futsal");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Hóquei em Patins");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Natação");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Voleibol");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "BTT");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Taekwondo");

            migrationBuilder.UpdateData(
                table: "Modality",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "Orientação");
        }
    }
}
