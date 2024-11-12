using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_Application.Migrations
{
    public partial class AddPatientFeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientFeedBack",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientRating",
                table: "Appointment",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientFeedBack",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PatientRating",
                table: "Appointment");
        }
    }
}
