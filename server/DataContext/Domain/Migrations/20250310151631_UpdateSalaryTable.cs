using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class UpdateSalaryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalaryBase",
                table: "Salaries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalAbsentNotCompleteShifts",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLateShifts",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalNumberOfShifts",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryBase",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "TotalAbsentNotCompleteShifts",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "TotalLateShifts",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "TotalNumberOfShifts",
                table: "Salaries");
        }
    }
}
