using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class Team : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModalityId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_AspNetUsers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Team_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Team_Modality_ModalityId",
                        column: x => x.ModalityId,
                        principalTable: "Modality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RoleClub",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 10, "Sócio" },
                    { 20, "Atleta" },
                    { 30, "Treinador" },
                    { 40, "Secretaria" },
                    { 50, "Administrador de Clube" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeamId",
                table: "AspNetUsers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ClubId",
                table: "Team",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ModalityId",
                table: "Team",
                column: "ModalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TrainerId",
                table: "Team",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Team_TeamId",
                table: "AspNetUsers",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Team_TeamId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TeamId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "RoleClub",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "AspNetUsers");

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
        }
    }
}
