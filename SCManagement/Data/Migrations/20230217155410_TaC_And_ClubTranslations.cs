using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class TaC_And_ClubTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Club",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClubTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Atribute = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubTranslations_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubTranslations_ClubId",
                table: "ClubTranslations",
                column: "ClubId");

            migrationBuilder.Sql("INSERT INTO ClubTranslations (ClubId, Language, Value, Atribute) SELECT Id, 'pt-PT', '', 'TermsAndConditions' FROM dbo.Club");
            migrationBuilder.Sql("INSERT INTO ClubTranslations (ClubId, Language, Value, Atribute) SELECT Id, 'en-US', '', 'TermsAndConditions' FROM dbo.Club");

            migrationBuilder.Sql("INSERT INTO ClubTranslations (ClubId, Language, Value, Atribute) SELECT Id, 'pt-PT', '', 'About' FROM dbo.Club");
            migrationBuilder.Sql("INSERT INTO ClubTranslations (ClubId, Language, Value, Atribute) SELECT Id, 'en-US', '', 'About' FROM dbo.Club");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubTranslations");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Club");
        }
    }
}
