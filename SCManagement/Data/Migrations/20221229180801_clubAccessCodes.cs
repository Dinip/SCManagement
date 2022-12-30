using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class clubAccessCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeClub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeClub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeClub_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodeClub_AspNetUsers_UsedByUserId",
                        column: x => x.UsedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CodeClub_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodeClub_RoleClub_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleClub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeClub_ClubId",
                table: "CodeClub",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeClub_Code",
                table: "CodeClub",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeClub_CreatedByUserId",
                table: "CodeClub",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeClub_RoleId",
                table: "CodeClub",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeClub_UsedByUserId",
                table: "CodeClub",
                column: "UsedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeClub");
        }
    }
}
