//using Auth.API.Constants;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace Auth.API.Migrations
//{
//    public partial class updateDB260225 : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<string>(
//                name: "Avatar",
//                table: "user",
//                type: "longtext",
//                nullable: true)
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.AddColumn<DateTime>(
//                name: "Dob",
//                table: "user",
//                type: "datetime",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "Phone",
//                table: "user",
//                type: "longtext",
//                nullable: true)
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.InsertData(
//            table: "Branches",
//            columns: new[] { "Id", "Name", "Address", "Phone", "Email", "Type", "IsActive", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted" },
//            values: new object[,]
//            {
//                { AppConstants.BRANCH_MAIN_ID, "Chi Nhánh Tổng", "123 Main St", "0123456789", "branch1@example.com", "Main", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false },
//                { AppConstants.BRANCH_SUB1_ID, "Chi Nhánh 1", "456 Elm St", "0987654321", "branch2@example.com", "Sub", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false },
//                { AppConstants.BRANCH_SUB2_ID, "Chi Nhánh 2", "456 Elm St", "0987654321", "branch2@example.com", "Sub", true, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, DateTime.UtcNow, AppConstants.SYSTEM_ROLE_GUID, false }
//            }
//            );

//            migrationBuilder.AddColumn<string>(
//            name: "Address",
//            table: "user",
//            type: "longtext",
//            nullable: true)
//            .Annotation("MySql:CharSet", "utf8mb4");

//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "Avatar",
//                table: "user");

//            migrationBuilder.DropColumn(
//                name: "Dob",
//                table: "user");

//            migrationBuilder.DropColumn(
//                name: "Phone",
//                table: "user");
//        }
//    }
//}
