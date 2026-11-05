using System;
using Domain.Constanst;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class AddStaticDataToWorkshift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GracePeriod",
                table: "WorkShifts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "WorkShifts",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.InsertData(
         table: "WorkShifts",
         columns: new[] { "Id", "Name", "StartTime", "EndTime", "GracePeriod", "Duration", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted" },
         values: new object[,]
         {
            { AppConstants.MORNING_SHIFT, "Morning", new TimeSpan(8, 0, 0), new TimeSpan(14, 0, 0), 10, new TimeSpan(6, 0, 0), DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, false },
            { AppConstants.AFTERNOON_SHIFT, "Afternoon", new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0), 10, new TimeSpan(6, 0, 0), DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, false },
            { AppConstants.EVENING_SHIFT, "Evening", new TimeSpan(20, 0, 0), new TimeSpan(2, 0, 0), 10, new TimeSpan(6, 0, 0), DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, DateTime.Now, AppConstants.SYSTEM_ROLE_GUID, false }
         }
     );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GracePeriod",
                table: "WorkShifts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "WorkShifts",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");
        }
    }
}
