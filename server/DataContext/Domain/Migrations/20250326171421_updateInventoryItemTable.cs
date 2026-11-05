using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class updateInventoryItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryReceiptId",
                table: "InventoryItem",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_InventoryReceiptId",
                table: "InventoryItem",
                column: "InventoryReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_InventoryReceipts_InventoryReceiptId",
                table: "InventoryItem",
                column: "InventoryReceiptId",
                principalTable: "InventoryReceipts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_InventoryReceipts_InventoryReceiptId",
                table: "InventoryItem");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_InventoryReceiptId",
                table: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "InventoryReceiptId",
                table: "InventoryItem");
        }
    }
}
