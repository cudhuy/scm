using Domain.Constanst;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class updateDB260225 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_MembershipRanks_RankId",
                table: "user");

            migrationBuilder.AlterColumn<Guid>(
                name: "RankId",
                table: "user",
                type: "char(36)",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: false)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<decimal>(
            name: "SalaryBase",
            table: "StaffInfos",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)");

            migrationBuilder.AddForeignKey(
                name: "FK_user_MembershipRanks_RankId",
                table: "user",
                column: "RankId",
                principalTable: "MembershipRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
             name: "Feedback",
                 columns: table => new
                 {
                     Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                     UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                     Content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                         .Annotation("MySql:CharSet", "utf8mb4"),
                     CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                     CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                     UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                     UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                     IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Feedback", x => x.Id);
                     table.ForeignKey(
                         name: "FK_Feedback_user_UserId",
                         column: x => x.UserId,
                         principalTable: "user",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade);
                 })
                 .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback",
                column: "UserId");



            migrationBuilder.InsertData(
            table: "Branches",
            columns: new[] { "Id", "Name", "Address", "Phone", "Email", "Type", "IsActive", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted" },
            values: new object[,]
            {
                { AppConstants.BRANCH_MAIN_GUID, "Chi Nhánh Tổng", "123 Main St", "0123456789", "branch1@example.com", "Main", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false },
                { AppConstants.BRANCH_SUB1_GUID, "Chi Nhánh 1", "456 Elm St", "0987654321", "branch2@example.com", "Sub", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false },
                { AppConstants.BRANCH_SUB2_GUID, "Chi Nhánh 2", "456 Elm St", "0987654321", "branch2@example.com", "Sub", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false }
            }
            );

            migrationBuilder.AddColumn<string>(
            name: "Address",
            table: "user",
            type: "longtext",
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_MembershipRanks_RankId",
                table: "user");



            migrationBuilder.AlterColumn<Guid>(
            name: "RankId",
            table: "user",
            type: "char(36)",
            nullable: true,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            collation: "ascii_general_ci",
            oldClrType: typeof(Guid),
            oldType: "char(36)",
            oldNullable: false)
            .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_user_MembershipRanks_RankId",
                table: "user",
                column: "RankId",
                principalTable: "MembershipRanks",
                principalColumn: "Id");
        }
    }
}
