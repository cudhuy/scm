using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class UpdateWorkShiftTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "WorkShifts",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GracePeriod",
                table: "WorkShifts",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "GracePeriod",
                table: "WorkShifts");
        }
    }
}
