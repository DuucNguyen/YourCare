using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_Application.Migrations
{
    public partial class AddAppointmentDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Appointment");
        }
    }
}
