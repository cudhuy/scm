using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class updateLowStockAlertTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "LowStockAlerts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_InventoryItemId",
                table: "LowStockAlerts",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_LowStockAlerts_InventoryItem_InventoryItemId",
                table: "LowStockAlerts",
                column: "InventoryItemId",
                principalTable: "InventoryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LowStockAlerts_InventoryItem_InventoryItemId",
                table: "LowStockAlerts");

            migrationBuilder.DropIndex(
                name: "IX_LowStockAlerts_InventoryItemId",
                table: "LowStockAlerts");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "LowStockAlerts");
        }
    }
}
