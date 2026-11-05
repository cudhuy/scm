using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class AddBranchIdToSalaryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Salaries",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_BranchId",
                table: "Salaries",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_Branches_BranchId",
                table: "Salaries",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_Branches_BranchId",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_BranchId",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Salaries");
        }
    }
}
