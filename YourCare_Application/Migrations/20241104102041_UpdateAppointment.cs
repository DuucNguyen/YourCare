using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_Application.Migrations
{
    public partial class UpdateAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Appointment",
                newName: "PatientNote");

            migrationBuilder.AddColumn<string>(
                name: "DoctorDianosis",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNote",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorDianosis",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "DoctorNote",
                table: "Appointment");

            migrationBuilder.RenameColumn(
                name: "PatientNote",
                table: "Appointment",
                newName: "Description");
        }
    }
}
