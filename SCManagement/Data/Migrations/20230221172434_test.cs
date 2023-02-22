using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCManagement.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnroolLimitDate",
                table: "Event",
                newName: "EnrollLimitDate");

            migrationBuilder.AlterColumn<float>(
                name: "Fee",
                table: "Event",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnrollLimitDate",
                table: "Event",
                newName: "EnroolLimitDate");

            migrationBuilder.AlterColumn<double>(
                name: "Fee",
                table: "Event",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
