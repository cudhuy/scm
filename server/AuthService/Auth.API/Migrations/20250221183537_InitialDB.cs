//using Auth.API.Constants;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace Auth.API.Migrations
//{
//    public partial class InitialDB : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AlterDatabase()
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.CreateTable(
//                name: "Branches",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Type = table.Column<string>(type: "enum('Main','Sub')", nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
//                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
//                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Branches", x => x.Id);
//                })
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.CreateTable(
//                name: "userrole",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    RoleName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
//                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
//                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_userrole", x => x.Id);
//                })
//                .Annotation("MySql:CharSet", "utf8mb4");


//            migrationBuilder.InsertData(
//                       table: "userrole",
//                       columns: new[] { "Id", "RoleName", "Description", "IsDeleted", "UpdatedBy", "CreatedBy", "CreatedAt", "UpdatedAt" },
//                       values: new object[,]
//                       {
//                {
//                    AppConstants.SYSTEM_ROLE_ID,
//                    "System",
//                    "Hệ thống",
//                    false,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc),
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc)
//                },
//                {
//                    AppConstants.ADMIN_ROLE_GUID,
//                    "Admin",
//                    "Quản trị hệ thống",
//                    false,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc),
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc)
//                },
//                {
//                    AppConstants.USER_ROLE_ID,
//                    "User",
//                    "Người dùng thông thường",
//                    false,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc),
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc)
//                },
//                {
//                    AppConstants.STAFF_ROLE_ID,
//                    "Staff",
//                    "Nhân viên",
//                    false,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc),
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc)
//                },
//                {
//                    AppConstants.MANAGER_ROLE_ID,
//                    "Manager",
//                    "Quản lý",
//                    false,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    AppConstants.SYSTEM_ROLE_ID,
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc),
//                    new DateTime(2024, 2, 6, 12, 0, 0, DateTimeKind.Utc)
//                }
//                       }
//                       );




//            migrationBuilder.CreateTable(
//                name: "user",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    PasswordHash = table.Column<byte[]>(type: "longblob", nullable: false),
//                    PasswordSalt = table.Column<byte[]>(type: "longblob", nullable: false),
//                    FullName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    IsVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
//                    RoleID = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
//                    Status = table.Column<string>(type: "enum('Active','Inactive')", nullable: true)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
//                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
//                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_user", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_User_Userrole",
//                        column: x => x.RoleID,
//                        principalTable: "userrole",
//                        principalColumn: "Id");
//                })
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.CreateTable(
//                name: "StaffInfos",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    Tittle = table.Column<string>(type: "longtext", nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    Type = table.Column<string>(type: "longtext", nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    SalaryBase = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
//                    BranchId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
//                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
//                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_StaffInfos", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_StaffInfos_Branches_BranchId",
//                        column: x => x.BranchId,
//                        principalTable: "Branches",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_StaffInfos_user_UserId",
//                        column: x => x.UserId,
//                        principalTable: "user",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                })
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.CreateTable(
//                name: "token",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UserID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    Token = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    TokenType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
//                        .Annotation("MySql:CharSet", "utf8mb4"),
//                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true),
//                    IsRevoked = table.Column<bool>(type: "tinyint(1)", nullable: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
//                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
//                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
//                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_token", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Token_User",
//                        column: x => x.UserID,
//                        principalTable: "user",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                })
//                .Annotation("MySql:CharSet", "utf8mb4");

//            migrationBuilder.CreateIndex(
//                name: "IX_StaffInfos_BranchId",
//                table: "StaffInfos",
//                column: "BranchId");

//            migrationBuilder.CreateIndex(
//                name: "IX_StaffInfos_UserId",
//                table: "StaffInfos",
//                column: "UserId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_token_UserID",
//                table: "token",
//                column: "UserID");

//            migrationBuilder.CreateIndex(
//                name: "IX_user_RoleID",
//                table: "user",
//                column: "RoleID");
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "StaffInfos");

//            migrationBuilder.DropTable(
//                name: "token");

//            migrationBuilder.DropTable(
//                name: "Branches");

//            migrationBuilder.DropTable(
//                name: "user");

//            migrationBuilder.DropTable(
//                name: "userrole");
//        }
//    }
//}
