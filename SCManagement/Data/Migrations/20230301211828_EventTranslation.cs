using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class EventTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Atribute = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTranslations_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTranslations_EventId",
                table: "EventTranslations",
                column: "EventId");


            migrationBuilder.Sql("INSERT INTO EventTranslations (EventId, Language, Value, Atribute) SELECT Id, 'pt-PT', Name, 'Name' FROM Event");
            migrationBuilder.Sql("INSERT INTO EventTranslations (EventId, Language, Value, Atribute) SELECT Id, 'en-US', Name, 'Name' FROM Event");

            migrationBuilder.Sql("INSERT INTO EventTranslations (EventId, Language, Value, Atribute) SELECT Id, 'pt-PT', Name, 'Details' FROM Event");
            migrationBuilder.Sql("INSERT INTO EventTranslations (EventId, Language, Value, Atribute) SELECT Id, 'en-US', Name, 'Details' FROM Event");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Event");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Event",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
