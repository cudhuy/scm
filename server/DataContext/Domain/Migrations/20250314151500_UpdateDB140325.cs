using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class UpdateDB140325 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipt_Ingredients_IngredientId",
                table: "InventoryReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipt_Inventories_InventoryId",
                table: "InventoryReceipt");

            migrationBuilder.DropTable(
                name: "TableOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryReceipt",
                table: "InventoryReceipt");

            migrationBuilder.RenameTable(
                name: "InventoryReceipt",
                newName: "InventoryReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryReceipt_InventoryId",
                table: "InventoryReceipts",
                newName: "IX_InventoryReceipts_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryReceipt_IngredientId",
                table: "InventoryReceipts",
                newName: "IX_InventoryReceipts_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryReceipts",
                table: "InventoryReceipts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipts_Ingredients_IngredientId",
                table: "InventoryReceipts",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipts_Inventories_InventoryId",
                table: "InventoryReceipts",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipts_Ingredients_IngredientId",
                table: "InventoryReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipts_Inventories_InventoryId",
                table: "InventoryReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryReceipts",
                table: "InventoryReceipts");

            migrationBuilder.RenameTable(
                name: "InventoryReceipts",
                newName: "InventoryReceipt");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryReceipts_InventoryId",
                table: "InventoryReceipt",
                newName: "IX_InventoryReceipt_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryReceipts_IngredientId",
                table: "InventoryReceipt",
                newName: "IX_InventoryReceipt_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryReceipt",
                table: "InventoryReceipt",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TableOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TableNumber = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TableOrders_OrderId",
                table: "TableOrders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipt_Ingredients_IngredientId",
                table: "InventoryReceipt",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipt_Inventories_InventoryId",
                table: "InventoryReceipt",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
