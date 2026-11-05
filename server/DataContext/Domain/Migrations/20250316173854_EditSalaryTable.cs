using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class EditSalaryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAbsentNotCompleteShifts",
                table: "Salaries",
                newName: "TotalNotCompleteShifts");

            migrationBuilder.AddColumn<int>(
                name: "TotalAbsentShifts",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAbsentShifts",
                table: "Salaries");

            migrationBuilder.RenameColumn(
                name: "TotalNotCompleteShifts",
                table: "Salaries",
                newName: "TotalAbsentNotCompleteShifts");
        }
    }
}
