using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_Application.Migrations
{
    public partial class AddUniqueConstrainTimetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timetable_DoctorID",
                table: "Timetable");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Timetable_DoctorID_Date_StartTime_EndTime",
                table: "Timetable",
                columns: new[] { "DoctorID", "Date", "StartTime", "EndTime" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timetable_DoctorID_Date_StartTime_EndTime",
                table: "Timetable");

            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "AspNetUsers",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timetable_DoctorID",
                table: "Timetable",
                column: "DoctorID");
        }
    }
}
