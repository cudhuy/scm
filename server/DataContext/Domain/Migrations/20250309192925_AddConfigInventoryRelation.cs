using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class AddConfigInventoryRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipt_Branches_BranchId",
                table: "InventoryReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_LowStockAlerts_Branches_BranchId",
                table: "LowStockAlerts");

            migrationBuilder.DropIndex(
                name: "IX_LowStockAlerts_BranchId",
                table: "LowStockAlerts");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReceipt_BranchId",
                table: "InventoryReceipt");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "LowStockAlerts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "InventoryReceipt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "LowStockAlerts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "InventoryReceipt",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_BranchId",
                table: "LowStockAlerts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReceipt_BranchId",
                table: "InventoryReceipt",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipt_Branches_BranchId",
                table: "InventoryReceipt",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LowStockAlerts_Branches_BranchId",
                table: "LowStockAlerts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
